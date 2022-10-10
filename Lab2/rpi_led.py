import RPi.GPIO as gp
import time

gp.setmode(gp.BCM)
gp.setup(17, gp.OUT)
pwm = gp.PWM(17, 100)
pwm.start(0)

try:
    while 1:
        for dc in range(0, 101, 5):
            print(dc)
            pwm.ChangeDutyCycle(dc)
            time.sleep(0.5)
except:
    pass
finally:
    pwm.stop()
    gp.cleanup()
