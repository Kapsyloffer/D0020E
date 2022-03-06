import numpy
import cv2


def maskRed1():
    return numpy.array([0, 50, 50]), numpy.array([10, 255, 255])


def maskRed2():
    return numpy.array([170, 50, 50]), numpy.array([180, 255, 255])


def maskRedTuned1():
    return numpy.array([2, 100, 100]), numpy.array([8, 255, 255])


def maskRedTuned2():
    return numpy.array([172, 100, 100]), numpy.array([178, 255, 255])


def maskRedTuned3():
    return numpy.array([3, 100, 120]), numpy.array([7, 255, 255])


def maskRedTuned4():
    return numpy.array([173, 100, 120]), numpy.array([177, 255, 255])


def maskGreen1():
    return numpy.array([50,100,120]), numpy.array([70,255,255])


def maskBlue():
    return numpy.array([100, 50, 50]), numpy.array([130, 255, 255])


def maskWhite():
    return numpy.array([0, 0, 0]), numpy.array([50, 0, 0])


def maskOrange1():
    return numpy.array([10, 150, 100]), numpy.array([20, 255, 255])


def maskMergedGen(frame, colorscheme, maskscheme1, maskscheme2):
    # It converts the BGR color space of image to HSV color space
    hsv = cv2.cvtColor(frame, colorscheme)
    lower1, upper1 = maskscheme1
    lower2, upper2 = maskscheme2
    mask1 = cv2.inRange(hsv, lower1, upper1)
    mask2 = cv2.inRange(hsv, lower2, upper2)
    return mask1 + mask2


def maskGen(frame, colorscheme):
    hsv = cv2.cvtColor(frame, colorscheme)
    lower, upper = maskRed2()
    return cv2.inRange(hsv, lower, upper)

    # Red
    # lower_red = numpy.array([0, 50, 50])
    # upper_red = numpy.array([10, 255 , 255])

    # mask0 = cv2.inRange(hsv,lower_blue,upper_blue)
    # mask0 = cv2.inRange(hsv, lower_red, upper_red)

    # upper mask(170 - 180) deprecated??
    # lower_red = numpy.array([170, 50, 50])
    # upper_red = numpy.array([180, 255, 255])
    # mask1 = cv2.inRange(hsv, lower_red, upper_red)

    # mask = mask0 + mask1
    # preparing the mask to overlay
    # mask = cv2.inRange(hsv, lower_red, upper_red)
    # return mask0
