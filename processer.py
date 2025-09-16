# -*- coding: utf-8 -*-

import os
import json
import signal
import time
import logging
from multiprocessing import Process, Queue
import queue

import logger
from game import GamePicklable

LOG = logging.getLogger(__name__)

PTaskQueue = Queue(20)
PResultQueue = Queue(20)
StopSignal = "mission_complete"


class Worker(Process):
    def __init__(self, wid, task_queue, result_queue):
        Process.__init__(self)
        self.wid = wid
        self.task_queue = task_queue
        self.result_queue = result_queue
        self.stop = False

    def sig_handler(self, sig, frame):
        LOG.warning("Worker(%03d) Caught signal: %s", self.wid, sig)
        self.stop = True

    def run(self):
        logger.config_logging(logger_name = "worker",
                              file_name = ("worker_%s" % self.wid + ".log"),
                              log_level = "INFO",
                              dir_name = "logs",
                              day_rotate = False,
                              when = "D",
                              interval = 1,
                              max_size = 20,
                              backup_count = 5,
                              console = True)
        LOG = logging.getLogger("worker")
        LOG.propagate = False
        LOG.info("Worker(%03d) start", self.wid)
        try:
            signal.signal(signal.SIGTERM, self.sig_handler)
            signal.signal(signal.SIGINT, self.sig_handler)
            while not self.stop:
                try:
                    task = self.task_queue.get(block = False)
                    think_mode, g, x, think_games = task
                    LOG.info("think_mode: %s, x: %s, games: %s", think_mode, x, think_games)
                    if think_mode == "mode1":
                        gg = GamePicklable()
                        stats_x = {g.red: 0, g.yellow: 0, g.empty: 0, "total": 0, "fast_over": {g.red: 42, g.yellow: 42, g.empty: 42}, "steps": {}}
                        for i in range(think_games):
                            n = 0
                            gg.copy_from(g)
                            while not gg.over:
                                gg.turn_random_place_disc()
                                n += 1
                            if gg.steps not in stats_x["steps"]:
                                stats_x["steps"][gg.steps] = True
                                stats_x[gg.win] += 1
                                if n < stats_x["fast_over"][gg.win]:
                                    stats_x["fast_over"][gg.win] = n
                    else:
                        stats_x = {g.red: 0, g.yellow: 0, g.empty: 0, "total": 0, "fast_over": {g.red: 42, g.yellow: 42, g.empty: 42}, "steps": {}}
                        g.recursive_turn_place_disc(stats_x, n = 0, target = think_games)
                    # LOG.info(stats_x)
                    self.result_queue.put((x, stats_x))
                except queue.Empty:
                    time.sleep(0.1)
        except Exception as e:
            LOG.exception(e)
        LOG.info("Worker(%03d) exit", self.wid)
