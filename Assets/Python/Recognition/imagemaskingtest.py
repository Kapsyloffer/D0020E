import cv2
import numpy

blue = numpy.array([255, 0, 0])
green = numpy.array([0, 255, 0])
red = numpy.array([0, 0, 255])

x = numpy.array([])
for i in range(0, 25):
    x = numpy.append(x, [i]).astype(int)
x = x.reshape((5, 5))

extract = x[3:5, 3:5]

startY = 3
endY = 5
startX = 0
endX = 5
roi = x[startY:endY, startX:endX]

while True:
    frame = cv2.imread('C:\img\streamer.jpg')
    cv2.imshow("original", frame)

    height = frame.shape[0]
    width = frame.shape[1]
    channels = frame.shape[2]
    midpoint = numpy.array([height // 2, width // 2])

    redbox = frame[midpoint[0]:height, midpoint[1]:width]
    blackbox = frame[0:midpoint[0], 0:midpoint[1]]

    # a mask is the same size as our image, but has only two pixel
    # values, 0 and 255 -- pixels with a value of 0 (background) are
    # ignored in the original image while mask pixels with a value of
    # 255 (foreground) are allowed to be kept

    croparea = numpy.zeros(frame.shape[:2], dtype="uint8")
    cv2.rectangle(croparea, (120, 80), (220, 180), 255, -1)
    cv2.imshow("Rectangular Mask", croparea)

    croparea2 = numpy.zeros(frame.shape[:2], dtype="uint8")
    cv2.circle(croparea2, (120, 80), 100, 255, -1)
    masked2 = cv2.bitwise_and(frame, frame, mask=croparea2)
    cv2.imshow("Circular mask",croparea2)

    masked = cv2.bitwise_and(frame, frame, mask=croparea)

    cv2.imshow("masked", masked)
    cv2.imshow("masked2",masked2)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cv2.destroyAllWindows()
