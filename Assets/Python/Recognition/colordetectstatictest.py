import cv2
import numpy
import packages.general.Functions as Functions
import packages.network.SocketO as SocketO
import packages.opencv.Drawtools as Drawtools
import packages.opencv.Masks as Masks
import packages.opencv.FrameObjectSingle as FrameO
import packages.opencv.TrackerO as TrackerO

# define the list of boundaries , BGR
lowerred = numpy.array([17, 15, 100])
upperred = numpy.array([50, 56, 200])
# boundaries = [
# (,  # Upper lower red
# ([86, 31, 4], [220, 88, 50]),
# ([25, 146, 190], [62, 174, 250]),
# ([103, 86, 65], [145, 133, 128])
# ]

while True:
    frame = cv2.imread('C:\img\slicetest.png')
    cv2.imshow("original", frame)
    # loop over the boundaries
    # for (lower, upper) in boundaries:
    # create NumPy arrays from the boundaries
    # lower = numpy.array(lower, dtype="uint8")
    # upper = numpy.array(upper, dtype="uint8")

    # find the colors within the specified boundaries and apply
    # the mask
    mask = cv2.inRange(frame, lowerred, upperred)
    output = cv2.bitwise_and(frame, frame, mask=mask)

    # show the images
    cv2.imshow("mask", mask)
    cv2.imshow("images", numpy.hstack([frame, output]))

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cv2.destroyAllWindows()
