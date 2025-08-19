import os
import time
import random
import pygame
import threading
from threading import Thread
from queue import Queue, Empty

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
    def __init__(self, task_queue, result_queue):
        StoppableThread.__init__(self)
        Thread.__init__(self)
        self.task_queue = task_queue
        self.result_queue = result_queue
        self.game = None

    def run(self):
        try:
            while True:
                if not self.stopped():
                    try:
                        turn = self.task_queue.get(block = False)
                        if self.game is not None and turn:
                            x = self.game.choose_best_move()
                            # self.game.dropping = True
                            y = self.game.drop_disc(x)
                            if y != -1:
                                self.game.dropping = True
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


class Disc(object):
    def __init__(self, color, x, y):
        self.x = x
        self.y = y
        self.color = color
        self.frame_n = 0

    def frames(self):
        return (self.y + 1) * 60

    def get_frame(self):
        self.frame_n += 1
        if self.frame_n <= (self.y + 1) * 2:
            return self.color, self.x, (self.frame_n - 1) * self.y / ((self.y + 1) * 2)
        return None, self.x, self.y


class Game(object):
    def __init__(self, think_games = 100, mode = "menu", task_queue = None, result_queue = None):
        self.title_font = pygame.font.Font("assets/BD_Cartoon_Shout.ttf", 72)
        self.item_font = pygame.font.Font("assets/BD_Cartoon_Shout.ttf", 36)
        self.stats_font = pygame.font.Font("assets/BD_Cartoon_Shout.ttf", 30)
        self.info_font = pygame.font.Font("assets/BD_Cartoon_Shout.ttf", 16)
        self.red = 1
        self.yellow = 2
        self.empty = 0
        self.board = [[self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty],
                      [self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty],
                      [self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty],
                      [self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty],
                      [self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty],
                      [self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty]]
        self.turn = self.red
        self.discs_counter = 0
        self.over = False
        self.win = self.empty
        self.think_games = think_games
        self.think_use_time = 0
        self.thinking = False
        self.mode = mode
        self.disc = Disc(self.red, 0, -1)
        self.dropping = False
        self.think = {self.red: 0, self.yellow: 0, self.empty: 0}
        self.stats = {self.red: 0, self.yellow: 0, self.empty: 0}
        self.menu_play_mode = ["play red", "play yellow", "two players"]
        self.menu_defficulty_mode = [10, 20, 30, 50, 100, 200, 500, 1000]
        self.menu_idx = 0
        self.menu_play_mode_idx = 0
        self.menu_difficulty_idx = 0
        self.cursor_x = 0
        self.task_queue = task_queue
        self.result_queue = result_queue

    def is_full(self):
        return self.discs_counter >= 42

    def available_place_y(self, x):
        for y in range(5, -1, -1):
            if self.board[y][x] == self.empty:
                return y
        return -1

    def available_place_xs(self):
        r = []
        for i, x in enumerate(self.board[0]):
            if x == self.empty:
                r.append(i)
        return r

    def place_disc(self, x, color):
        y = self.available_place_y(x)
        if y != -1:
            self.board[y][x] = color
            self.discs_counter += 1
            return y
        return -1

    def is_same3(self, v1, v2, v3, color):
        return v1 == v2 == v3 == color

    def check_offensive_move23(self, color):
        r = [0, 0, 0, 0, 0, 0, 0]
        for x in range(7):
            y = self.available_place_y(x)
            if y != -1:
                self.board[y][x] = color
                for xx in range(max(0, x - 2), min(x + 1, 5)):
                    if self.is_same3(self.board[y][xx], self.board[y][xx + 1], self.board[y][xx + 2], color):
                        r[x] += 1
                if y <= 3 and self.is_same3(self.board[y][x], self.board[y + 1][x], self.board[y + 2][x], color):
                    r[x] += 1
                for d in range(3):
                    if x - d >= 0 and x - d + 2 <= 6 and y - d >= 0 and y - d + 2 <= 5:
                        if self.is_same3(self.board[y - d][x - d], self.board[y - d + 1][x - d + 1], self.board[y - d + 2][x - d + 2], color):
                            r[x] += 1
                    if x - d >= 0 and x - d + 2 <= 6 and y + d <= 5 and y + d - 2 >= 0:
                        if self.is_same3(self.board[y + d][x - d], self.board[y + d - 1][x - d + 1], self.board[y + d - 2][x - d + 2], color):
                            r[x] += 1
                self.board[y][x] = self.empty
        return r

    def is_line5(self, v1, v2, v3, v4, v5, color):
        return v1 == v5 == self.empty and v2 == v3 == v4 == color

    def check_offensive_win_move23(self, color):
        r = [0, 0, 0, 0, 0, 0, 0]
        for x in range(7):
            y = self.available_place_y(x)
            if y != -1:
                self.board[y][x] = color
                for xx in range(max(0, x - 3), min(x, 3)):
                    if self.is_line5(self.board[y][xx], self.board[y][xx + 1], self.board[y][xx + 2], self.board[y][xx + 3], self.board[y][xx + 4], color) and (y == self.available_place_y(xx) == self.available_place_y(xx + 4)):
                        r[x] += 1
                for d in range(4):
                    if x - d >= 0 and x - d + 4 <= 6 and y - d >= 0 and y - d + 4 <= 5:
                        if self.is_line5(self.board[y - d][x - d], self.board[y - d + 1][x - d + 1], self.board[y - d + 2][x - d + 2], self.board[y - d + 3][x - d + 3], self.board[y - d + 4][x - d + 4], color) and (self.available_place_y(x - d) == y - d and self.available_place_y(x - d + 4) == y - d + 4):
                            r[x] += 1
                    if x - d >= 0 and x - d + 4 <= 6 and y + d <= 5 and y + d - 4 >= 0:
                        if self.is_line5(self.board[y + d][x - d], self.board[y + d - 1][x - d + 1], self.board[y + d - 2][x - d + 2], self.board[y + d - 3][x - d + 3], self.board[y + d - 4][x - d + 4], color)  and (self.available_place_y(x - d) == y + d and self.available_place_y(x - d + 4) == y + d - 4):
                            r[x] += 1
                self.board[y][x] = self.empty
        return r

    def is_any_line4(self, v1, v2, v3, v4, color):
        return ((v1 == v2 == v3 == color and v4 == self.empty) or
                (v1 == v3 == v4 == color and v2 == self.empty) or
                (v2 == v3 == v4 == color and v1 == self.empty) or
                (v1 == v2 == v4 == color and v3 == self.empty))

    def check_offensive_lock_move23(self, color):
        r = [0, 0, 0, 0, 0, 0, 0]
        for x in range(7):
            y = self.available_place_y(x)
            if y != -1:
                self.board[y][x] = color
                for xx in range(max(0, x - 3), min(x + 1, 4)):
                    if self.is_any_line4(self.board[y][xx], self.board[y][xx + 1], self.board[y][xx + 2], self.board[y][xx + 3], color):
                        r[x] += 1
                for d in range(4):
                    if x - d >= 0 and x - d + 3 <= 6 and y - d >= 0 and y - d + 3 <= 5:
                        if self.is_any_line4(self.board[y - d][x - d], self.board[y - d + 1][x - d + 1], self.board[y - d + 2][x - d + 2], self.board[y - d + 3][x - d + 3], color):
                            r[x] += 1
                    if x - d >= 0 and x - d + 3 <= 6 and y + d <= 5 and y + d - 3 >= 0:
                        if self.is_any_line4(self.board[y + d][x - d], self.board[y + d - 1][x - d + 1], self.board[y + d - 2][x - d + 2], self.board[y + d - 3][x - d + 3], color):
                            r[x] += 1
                self.board[y][x] = self.empty
        return r

    def is_same4(self, v1, v2, v3, v4, color):
        return v1 == v2 == v3 == v4 == color

    def check_offensive_move34(self, color):
        r = [0, 0, 0, 0, 0, 0, 0]
        for x in range(7):
            y = self.available_place_y(x)
            if y != -1:
                self.board[y][x] = color
                for xx in range(max(0, x - 3), min(x + 1, 4)):
                    if self.is_same4(self.board[y][xx], self.board[y][xx + 1], self.board[y][xx + 2], self.board[y][xx + 3], color):
                        r[x] += 1
                if y <= 2 and self.is_same4(self.board[y][x], self.board[y + 1][x], self.board[y + 2][x], self.board[y + 3][x], color):
                    r[x] += 1
                for d in range(4):
                    if x - d >= 0 and x - d + 3 <= 6 and y - d >= 0 and y - d + 3 <= 5:
                        if self.is_same4(self.board[y - d][x - d], self.board[y - d + 1][x - d + 1], self.board[y - d + 2][x - d + 2], self.board[y - d + 3][x - d + 3], color):
                            r[x] += 1
                    if x - d >= 0 and x - d + 3 <= 6 and y + d <= 5 and y + d - 3 >= 0:
                        if self.is_same4(self.board[y + d][x - d], self.board[y + d - 1][x - d + 1], self.board[y + d - 2][x - d + 2], self.board[y + d - 3][x - d + 3], color):
                            r[x] += 1
                self.board[y][x] = self.empty
        return r

    def is_line4(self, v1, v2, v3, v4):
        return v1 != self.empty and v1 == v2 == v3 == v4

    def check_status(self, x, y):
        for xx in range(max(0, x - 3), min(x + 1, 4)):
            if self.is_line4(self.board[y][xx], self.board[y][xx + 1], self.board[y][xx + 2], self.board[y][xx + 3]):
                return self.board[y][xx]
        if y <= 2 and self.is_line4(self.board[y][x], self.board[y + 1][x], self.board[y + 2][x], self.board[y + 3][x]):
            return self.board[y][x]
        for d in range(4):
            if x - d >= 0 and x - d + 3 <= 6 and y - d >= 0 and y - d + 3 <= 5:
                if self.is_line4(self.board[y - d][x - d], self.board[y - d + 1][x - d + 1], self.board[y - d + 2][x - d + 2], self.board[y - d + 3][x - d + 3]):
                    return self.board[y - d][x - d]
            if x - d >= 0 and x - d + 3 <= 6 and y + d <= 5 and y + d - 3 >= 0:
                if self.is_line4(self.board[y + d][x - d], self.board[y + d - 1][x - d + 1], self.board[y + d - 2][x - d + 2], self.board[y + d - 3][x - d + 3]):
                    return self.board[y + d][x - d]
        return self.empty

    def drop_disc(self, x):
        y = self.available_place_y(x)
        if y != -1:
            self.disc.color = self.turn
            self.disc.x = x
            self.disc.y = y
            self.disc.frame_n = 0
            return y
        return -1

    def turn_place_disc(self, x):
        y = self.place_disc(x, self.turn)
        if y != -1:
            if self.turn == self.red:
                self.turn = self.yellow
            else:
                self.turn = self.red
            win = self.check_status(x, y)
            if win != self.empty:
                self.over = True
                self.win = win
                self.stats[self.win] += 1
            if self.is_full():
                if not self.over:
                    self.over = True
                    self.win = self.empty
                    self.stats[self.win] += 1
            return True
        return False

    def check_win_move(self):
        m34 = self.check_offensive_move34(self.turn)
        m34_max = max(m34)
        if m34_max > 0:
            return m34.index(m34_max)
        # for x in range(7):
        #     if m34[x] > 0:
        #         return x
        return -1

    def check_defensive_move(self):
        opponent = self.red if self.turn == self.yellow else self.yellow
        # m23 = self.check_offensive_move23(opponent)
        m34 = self.check_offensive_move34(opponent)
        m3 = self.check_offensive_win_move23(opponent)
        # m3l = self.check_offensive_lock_move23(opponent)
        m34_max = max(m34)
        if m34_max > 0:
            idx = [i for i, v in enumerate(m34) if v == m34_max]
            return random.choice(idx)
        m3_max = max(m3)
        if m3_max > 0:
            idx = [i for i, v in enumerate(m3) if v == m3_max]
            return random.choice(idx)
        # for x in range(7):
        #     # if m23[x] > 0 and m34[x] > 0:
        #     #     return x
        #     if m34[x] > 0:
        #         return x
        # for x in range(7):
        #     if m3[x] > 0:
        #         return x
        # for x in range(7):
        #     if m3l[x] > 0:
        #         return x
        return -1

    def turn_random_place_disc(self):
        x = self.check_win_move()
        if x == -1:
            x = self.check_defensive_move()
            if x == -1:
                xs = self.available_place_xs()
                x = random.choice(xs)
        return self.turn_place_disc(x)

    def choose_best_move(self):
        t = time.time()
        if self.discs_counter < 2:
            return 3
        best_x = self.check_win_move()
        if best_x == -1:
            best_x = self.check_defensive_move()
        if best_x == -1:
            xs = self.available_place_xs()
            stats = {}
            g = Game()
            for x in xs:
                stats[x] = {self.red: 0, self.yellow: 0, self.empty: 0}
                for i in range(self.think_games):
                    g.copy_from(self)
                    g.turn_place_disc(x)
                    while not g.over:
                        g.turn_random_place_disc()
                    stats[x][g.win] += 1
            max_win = stats[xs[0]][self.turn]
            best_x = xs[0]
            for x in xs[1:]:
                if stats[x][self.turn] > max_win: # or (stats[x][self.turn] == max_win and stats[x][self.empty] > stats[best_x][self.empty]):
                    max_win = stats[x][self.turn]
                    best_x = x
            self.think[self.red] = stats[best_x][self.red]
            self.think[self.yellow] = stats[best_x][self.yellow]
            self.think[self.empty] = stats[best_x][self.empty]
        self.think_use_time = time.time() - t
        return best_x

    def restart(self):
        self.turn = self.red
        self.discs_counter = 0
        self.board = [[self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty],
                      [self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty],
                      [self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty],
                      [self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty],
                      [self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty],
                      [self.empty, self.empty, self.empty, self.empty, self.empty, self.empty, self.empty]]
        self.over = False
        self.win = self.empty
        self.think[self.red] = 0
        self.think[self.yellow] = 0
        self.think[self.empty] = 0
        self.think_use_time = 0

    def copy_from(self, game):
        self.turn = game.turn
        self.discs_counter = game.discs_counter
        self.board = [list(row) for row in game.board]
        self.over = game.over
        self.win = game.win

    def render_menu(self, window):
        green = (81, 146, 3)
        offset_x = 0
        offset_y = 0
        y = 100
        window.fill((54, 88, 156))
        title = self.title_font.render("Connect Four", True, (0, 0, 0))
        x = (window.get_width() - title.get_width()) // 2
        window.blit(title, (x, y))
        y += (250 * title.get_height()) // 100

        play_mode = self.item_font.render(("< %s >" if self.menu_idx == 0 else "%s") % self.menu_play_mode[self.menu_play_mode_idx], True, (0, 0, 0))
        x = (window.get_width() - play_mode.get_width()) // 2
        window.blit(play_mode, (x, y))
        y += (150 * play_mode.get_height()) // 100

        difficulty_mode = self.item_font.render(("< difficulty: %s >" if self.menu_idx == 1 else "difficulty: %s") % self.menu_defficulty_mode[self.menu_difficulty_idx], True, (0, 0, 0))
        x = (window.get_width() - difficulty_mode.get_width()) // 2
        window.blit(difficulty_mode, (x, y))
        y += (150 * difficulty_mode.get_height()) // 100 + 100

        help_info = self.info_font.render("up or down to switch options", True, green)
        x = (window.get_width() - help_info.get_width()) // 2
        window.blit(help_info, (x, y))
        y += (150 * help_info.get_height()) // 100

        help_info = self.info_font.render("left or right to change values", True, green)
        x = (window.get_width() - help_info.get_width()) // 2
        window.blit(help_info, (x, y))
        y += (150 * help_info.get_height()) // 100

        help_info = self.info_font.render("enter to play", True, green)
        x = (window.get_width() - help_info.get_width()) // 2
        window.blit(help_info, (x, y))
        y += (150 * help_info.get_height()) // 100

        help_info = self.info_font.render("esc to quit", True, green)
        x = (window.get_width() - help_info.get_width()) // 2
        window.blit(help_info, (x, y))

    def process_menu_input(self, quit):
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                quit()
                break
            elif event.type == pygame.KEYDOWN:
                if event.key == pygame.K_ESCAPE:
                    quit()
                elif event.key == pygame.K_RETURN:
                    self.mode = self.menu_play_mode[self.menu_play_mode_idx]
                    self.think_games = self.menu_defficulty_mode[self.menu_difficulty_idx]
                    self.restart()
                    self.stats = {self.red: 0, self.yellow: 0, self.empty: 0}
                    if self.mode == "play yellow":
                        self.thinking = True
                        self.task_queue.put(self.turn, block = True)
                elif event.key == pygame.K_LEFT:
                    if self.menu_idx == 0:
                        self.menu_play_mode_idx -= 1
                        if self.menu_play_mode_idx <= 0:
                            self.menu_play_mode_idx = 0
                    elif self.menu_idx == 1:
                        self.menu_difficulty_idx -= 1
                        if self.menu_difficulty_idx <= 0:
                            self.menu_difficulty_idx = 0
                elif event.key == pygame.K_RIGHT:
                    if self.menu_idx == 0:
                        self.menu_play_mode_idx += 1
                        if self.menu_play_mode_idx >= len(self.menu_play_mode) - 1:
                            self.menu_play_mode_idx = len(self.menu_play_mode) - 1
                    elif self.menu_idx == 1:
                        self.menu_difficulty_idx += 1
                        if self.menu_difficulty_idx >= len(self.menu_defficulty_mode) - 1:
                            self.menu_difficulty_idx = len(self.menu_defficulty_mode) - 1
                elif event.key == pygame.K_UP:
                    self.menu_idx -= 1
                    if self.menu_idx < 0:
                        self.menu_idx = 0
                elif event.key == pygame.K_DOWN:
                    self.menu_idx += 1
                    if self.menu_idx >= 1:
                        self.menu_idx = 1

    def render_game(self, window):
        red = (180, 53, 53)
        yellow = (214, 199, 11)
        green = (81, 146, 3)
        offset_x = 0
        offset_y = 0
        window.fill((220,220,220))
        red_disc = pygame.image.load("assets/disc-red.png")
        yellow_disc = pygame.image.load("assets/disc-yellow.png")
        red_disc_small = pygame.image.load("assets/disc-red-small.png")
        yellow_disc_small = pygame.image.load("assets/disc-yellow-small.png")
        if self.dropping:
            color, x, y = self.disc.get_frame()
            if color == self.red:
                window.blit(red_disc_small, (offset_x + x * 128, offset_y + y * 128))
            elif color == self.yellow:
                window.blit(yellow_disc_small, (offset_x + x * 128, offset_y + y * 128))
            else:
                self.turn_place_disc(x)
                self.dropping = False
                if not self.over and ((self.mode == "play red" and self.turn == self.yellow) or (self.mode == "play yellow" and self.turn == self.red)):
                    self.thinking = True
                    self.task_queue.put(self.turn, block = True)

        for y in range(6):
            for x in range(7):
                if self.board[y][x] == self.red:
                    window.blit(red_disc_small, (offset_x + x * 128, offset_y + y * 128))
                elif self.board[y][x] == self.yellow:
                    window.blit(yellow_disc_small, (offset_x + x * 128, offset_y + y * 128))
                b = pygame.image.load("assets/board-part.png")
                window.blit(b, (offset_x + x * 128, offset_y + y * 128))
                pygame.draw.rect(window, red if self.turn == self.red else yellow, (offset_x + self.cursor_x * 128, offset_y, 128, 768), 5)
                window.blit(red_disc_small, (offset_x + 7 * 128, offset_y + 0 * 128))

                red_win_stats = self.stats_font.render(": %s %s" % (self.stats[self.red], "WIN" if self.over and self.win == self.red else ""), True, red)
                window.blit(red_win_stats, (offset_x + 7 * 128 + 128, offset_y + 0 * 128 + 48))
                
                window.blit(yellow_disc_small, (offset_x + 7 * 128, offset_y + 1 * 128))
                tie_stats = self.stats_font.render(": %s %s" % (self.stats[self.empty], "TIE" if self.over and self.win == self.empty else ""), True, (0, 0, 0))
                window.blit(tie_stats, (offset_x + 7 * 128 + 128, offset_y + 0 * 128 + 112))

                yellow_win_stats = self.stats_font.render(": %s %s" % (self.stats[self.yellow], "WIN" if self.over and self.win == self.yellow else ""), True, yellow)
                window.blit(yellow_win_stats, (offset_x + 7 * 128 + 128, offset_y + 1 * 128 + 48))

                if self.thinking:
                    if self.turn == self.red:
                        thinking = self.stats_font.render("thinking", True, red)
                        window.blit(thinking, (offset_x + 7 * 128 + 20, offset_y + 2 * 128 + 10))
                    else:
                        thinking = self.stats_font.render("thinking", True, yellow)
                        window.blit(thinking, (offset_x + 7 * 128 + 20, offset_y + 2 * 128 + 10))

                think_title = self.stats_font.render("CPU think:", True, (0, 0, 0))
                window.blit(think_title, (offset_x + 7 * 128 + 5, offset_y + 3 * 128))
                think_time = self.stats_font.render(" use: %.2fms" % (self.think_use_time * 1000.0), True, (0, 0, 0))
                window.blit(think_time, (offset_x + 7 * 128 + 5, offset_y + 3 * 128 + 48))
                think_red = self.stats_font.render(" red: %d/%d" % (self.think[self.red], self.think_games), True, (0, 0, 0))
                window.blit(think_red, (offset_x + 7 * 128 + 5, offset_y + 3 * 128 + 96))
                think_yellow = self.stats_font.render(" yellow: %d/%d" % (self.think[self.yellow], self.think_games), True, (0, 0, 0))
                window.blit(think_yellow, (offset_x + 7 * 128 + 5, offset_y + 3 * 128 + 144))
                think_tie = self.stats_font.render(" tie: %d/%d" % (self.think[self.empty], self.think_games), True, (0, 0, 0))
                window.blit(think_tie, (offset_x + 7 * 128 + 5, offset_y + 3 * 128 + 192))

                help_info = self.info_font.render("left or right to switch column", True, green)
                window.blit(help_info, (offset_x + 7 * 128 + 5, offset_y + 3 * 128 + 300))
                help_info = self.info_font.render("enter to place disc", True, green)
                window.blit(help_info, (offset_x + 7 * 128 + 5, offset_y + 3 * 128 + 320))
                help_info = self.info_font.render("r to restart", True, green)
                window.blit(help_info, (offset_x + 7 * 128 + 5, offset_y + 3 * 128 + 340))
                help_info = self.info_font.render("esc back to menu", True, green)
                window.blit(help_info, (offset_x + 7 * 128 + 5, offset_y + 3 * 128 + 360))


    def process_game_input(self, quit):
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                quit()
                break
            elif event.type == pygame.KEYDOWN:
                if event.key == pygame.K_ESCAPE:
                    self.mode = "menu"
                elif event.key == pygame.K_RETURN:
                    if not self.over and not self.thinking and not self.dropping:
                        y = self.drop_disc(self.cursor_x)
                        if y != -1:
                            self.dropping = True
                elif event.key == pygame.K_r:
                    self.restart()
                    if self.mode == "play yellow":
                        self.thinking = True
                        self.task_queue.put(self.turn, block = True)
                elif event.key == pygame.K_LEFT:
                    self.cursor_x -= 1
                    if self.cursor_x <= 0:
                        self.cursor_x = 0
                elif event.key == pygame.K_RIGHT:
                    self.cursor_x += 1
                    if self.cursor_x >= 6:
                        self.cursor_x = 6


class UserInterface(object):
    def __init__(self, think_thread, task_queue, result_queue):
        pygame.init()
        self.window = pygame.display.set_mode((1280, 768)) # , pygame.RESIZABLE)
        pygame.display.set_caption("Connect Four")
        pygame.display.set_icon(pygame.image.load("assets/disc-red.png"))

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
    think = ThinkThread(TaskQueue, ResultQueue)
    think.start()
    UserInterface = UserInterface(think, TaskQueue, ResultQueue)
    UserInterface.run()
    think.join()
    pygame.quit()