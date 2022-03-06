import cv2
import numpy

blue = numpy.array([255, 0, 0])
green = numpy.array([0, 255, 0])
red = numpy.array([0, 0, 255])

while True:
    frame = cv2.imread('C:\img\slicetest.png')
    # get dimensions of image
    dimensions = frame.shape

    # height, width, number of channels in image
    height = frame.shape[0]
    width = frame.shape[1]
    channels = frame.shape[2]

    midpoint = numpy.array([height // 2, width // 2])
    midy = midpoint[0]
    midx = midpoint[1]
    # for x in range(midpoint[0], midpoint[0] + 100):
    # frame[x, x] = blue
    topleft = frame[0:midy, 0:midx]
    topright = frame[0:midy, midx:width]
    bottomright = frame[midy:height, midx:width]
    bottomleft = frame[midy:height, 0:midx]

    for i in range(height - 200, height):
        frame[i - 200, i] = red
        frame[0, i] = blue

    # Y, X = numpy.where(numpy.all(topright == [0, 255, 0], axis=2))
    # frame[Y, X] = red

    #tlY, tlX = numpy.where(numpy.all(topleft == [0, 0, 0], axis=2))
    #topleft[tlY, tlX] = blue

    for j in range(0, midpoint[0]):
        for h in range(0,midpoint[0]):
            frame[h, j] = green
            frame[j, h] = red

    cv2.imshow("slicetest", frame)
    cv2.imshow("top left", topleft)
    cv2.imshow("top right", topright)
    cv2.imshow("bottom left", bottomleft)
    cv2.imshow("bottom right", bottomright)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cv2.destroyAllWindows()
