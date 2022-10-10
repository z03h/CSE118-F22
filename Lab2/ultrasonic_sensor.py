import RPi.GPIO as gp
import time

gp.setmode(gp.BCM)
gp.setup(17, gp.OUT)
gp.setup(18, gp.IN)

start = end = 0
try:
    while 1:
        gp.output(17, True)
        time.sleep(0.00001)
        gp.output(17, False)

        while gp.input(18) == 0:
            start = time.time()
        while gp.input(18) == 1:
            end = time.time()
        tt = 343 * (end-start) * 2
        print(f'{tt} meters')
except:
    pass
finally:
    gp.cleanup()
