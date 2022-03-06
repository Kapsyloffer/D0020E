import cv2
import numpy
import packages.general.Functions as Functions
import packages.network.SocketO as SocketO
import packages.opencv.Drawtools as Drawtools
import packages.opencv.Masks as Masks
import packages.opencv.FrameObjectSingle as FrameO
import packages.opencv.TrackerO as TrackerO

center = FrameO.FrameObjectSingle("center")
left = FrameO.FrameObjectSingle("left")
zero = FrameO.FrameObjectSingle("none")
blue = numpy.array([255, 0, 0])
while True:

    # lower_red = numpy.array([0, 0, 220])
    # upper_red = numpy.array([10, 10, 255])
    # mask = cv2.inRange(c,lower_red,upper_red)
    # coord = cv2.findNonZero(c)
    # print(coord)
    # cv2.imshow("mask",mask)
    #cv2.imshow("c", c)

    # print(X, Y)

    # print(z)
    # cv2.waitKey(5000)

    # center.ops("show frame")
    # left.ops("show frame")
    # zero.ops("show frame")
    # image = cv2.imread('C:\img\center.png')
    # cv2.imshow('image window', image)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cv2.destroyAllWindows()
# masked = FrameO.FrameObjectSingle("masked")


# center.ops("set input file img", 'C:\img\center.png')
# left.ops("set input file img", 'C:\img\left.png')
# zero.ops("set input file img", 'C:\img\zero.png')
# c = center.frame
# l = left.frame
# z = zero.frame