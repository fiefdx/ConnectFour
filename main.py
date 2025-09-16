import os
import sys
import traceback
import logging
import math
import time
import random
import pygame
import threading
from threading import Thread
from queue import Queue, Empty

import logger
from game import Game
from processer import Worker, PTaskQueue, PResultQueue

LOG = logging.getLogger(__name__)

__version__ = "1.3.2"

os.environ['SDL_VIDEO_CENTERED'] = '1'

TaskQueue = Queue(5)
ResultQueue = Queue(5)


class StoppableThread(Thread):
    def __init__(self):
        super(StoppableThread, self).__init__()
        self._stop_event = threading.Event()

    def stop(self):
        self._stop_event.set()

    def stopped(self):
        return self._stop_event.is_set()


class ThinkThread(StoppableThread):
    def __init__(self, task_queue, result_queue, worker_task_queue, worker_result_queue):
        StoppableThread.__init__(self)
        Thread.__init__(self)
        self.task_queue = task_queue
        self.result_queue = result_queue
        self.worker_task_queue = worker_task_queue
        self.worker_result_queue = worker_result_queue
        self.game = None

    def run(self):
        try:
            while True:
                if not self.stopped():
                    try:
                        turn = self.task_queue.get(block = False)
                        if self.game is not None and turn:
                            if turn == 1:
                                self.game.think_games = self.game.think_games_red
                                self.game.think_mode = self.game.think_mode_red
                            elif turn == 2:
                                self.game.think_games = self.game.think_games_yellow
                                self.game.think_mode = self.game.think_mode_yellow
                            # x = self.game.choose_best_move()
                            t = time.time()
                            best_x = self.game.choose_immediate_move()
                            if best_x == -1:
                                n = 0
                                xs = []
                                for task in self.game.iter_best_move_tasks():
                                    xs.append(task[2])
                                    self.worker_task_queue.put(task)
                                    n += 1
                                stats = {}
                                for i in range(n):
                                    x, s = self.worker_result_queue.get()
                                    stats[x] = s
                                # LOG.info("all stats:\n%s", stats)
                                max_win = stats[xs[0]][self.game.turn]
                                fast_lose = stats[xs[0]]["fast_over"][self.game.red if self.game.turn == self.game.yellow else self.game.yellow]
                                best_x = xs[0]
                                for x in xs[1:]:
                                    if fast_lose == 1:
                                        max_win = stats[x][self.game.turn]
                                        fast_lose = stats[x]["fast_over"][self.game.red if self.game.turn == self.game.yellow else self.game.yellow]
                                        best_x = x
                                    else:
                                        if stats[x][self.game.turn] > max_win and stats[x]["fast_over"][self.game.red if self.game.turn == self.game.yellow else self.game.yellow] != 1: # or (stats[x][self.turn] == max_win and stats[x][self.empty] > stats[best_x][self.empty]):
                                            max_win = stats[x][self.game.turn]
                                            fast_lose = stats[x]["fast_over"][self.game.red if self.game.turn == self.game.yellow else self.game.yellow]
                                            best_x = x
                                self.game.think[self.game.red] = stats[best_x][self.game.red]
                                self.game.think[self.game.yellow] = stats[best_x][self.game.yellow]
                                self.game.think[self.game.empty] = stats[best_x][self.game.empty]
                            self.game.think_use_time = time.time() - t

                            # self.game.dropping = True
                            y = self.game.drop_disc(best_x)
                            if y != -1:
                                self.game.dropping = True
                                self.game.cursor_x = best_x
                            # self.game.turn_place_disc(x)
                            self.game.thinking = False
                        else:
                            time.sleep(0.1)
                    except Empty:
                        time.sleep(0.1)
                else:
                    break
        except Exception as e:
            print(e)


class UserInterface(object):
    def __init__(self, think_thread, task_queue, result_queue):
        pygame.init()
        pygame.mixer.init()
        self.window = pygame.display.set_mode((1280, 768), pygame.FULLSCREEN | pygame.SCALED) # , pygame.RESIZABLE)
        pygame.display.set_caption("Connect Four - v%s" % __version__)
        pygame.display.set_icon(pygame.image.load("assets/icon.png"))

        self.game = Game(task_queue = task_queue, result_queue = result_queue)
        self.think_thread = think_thread
        self.think_thread.game = self.game

        self.clock = pygame.time.Clock()
        self.running = True

    def quit(self):
        self.think_thread.stop()
        self.running = False

    def process_input(self):
        if self.game.mode == "menu":
            self.game.process_menu_input(self.quit)
        else:
            self.game.process_game_input(self.quit)

    def render(self):
        if self.game.mode == "menu":
            self.game.render_menu(self.window)
        else:
            self.game.render_game(self.window)
        pygame.display.update()

    def run(self):
        while self.running:
            self.process_input()
            self.render()
            self.clock.tick(60)

if __name__ == "__main__":
    logger.config_logging(file_name = "game.log",
                          log_level = "INFO",
                          dir_name = "logs",
                          day_rotate = False,
                          when = "D",
                          interval = 1,
                          max_size = 20,
                          backup_count = 5,
                          console = True)
    LOG.info("start game")
    process_num = 4
    if len(sys.argv) > 1:
        process_num = int(sys.argv[1])
    for i in range(process_num):
        w = Worker(i, PTaskQueue, PResultQueue)
        w.daemon = True
        w.start()
    think = ThinkThread(TaskQueue, ResultQueue, PTaskQueue, PResultQueue)
    think.start()
    UserInterface = UserInterface(think, TaskQueue, ResultQueue)
    UserInterface.run()
    think.join()
    pygame.quit()
    LOG.info("end game")