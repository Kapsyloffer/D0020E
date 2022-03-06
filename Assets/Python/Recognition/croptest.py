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
    frame = cv2.imread('C:\img\slicetest.png')
    cv2.imshow("original", frame)

    height = frame.shape[0]
    width = frame.shape[1]
    channels = frame.shape[2]
    midpoint = numpy.array([height // 2, width // 2])

    redbox = frame[midpoint[0]:height, midpoint[1]:width]
    blackbox = frame[0:midpoint[0], 0:midpoint[1]]

    # draw a circle
    circle = numpy.zeros((300, 300), dtype="uint8")
    cv2.circle(circle, (150, 150), 150, 255, -1)

    # draw a rectangle
    rectangle = numpy.zeros((300, 300), dtype="uint8")
    cv2.rectangle(rectangle, (25, 25), (275, 275), 255, -1)

    # a bitwise 'AND' is only 'True' when both inputs have a value that
    # is 'ON' -- in this case, the cv2.bitwise_and function examines
    # every pixel in the rectangle and circle; if *BOTH* pixels have a
    # value greater than zero then the pixel is turned 'ON' (i.e., 255)
    # in the output image; otherwise, the output value is set to
    # 'OFF' (i.e., 0)

    # the bitwise 'NOT' inverts the values of the pixels;
    # pixels with a value of 255 become 0, and pixels with a value of 0
    # become 255

    maskedand = cv2.bitwise_and(rectangle, circle)
    maskedxor = cv2.bitwise_xor(rectangle, circle)
    maskednot = cv2.bitwise_not(circle)

    # cv2.imshow("redbox",redbox)
    # cv2.imshow("blackbox" ,blackbox)
    cv2.imshow("Circle", circle)
    cv2.imshow("Rectangle", rectangle)
    cv2.imshow("maskedand", maskedand)
    cv2.imshow("maskedxor", maskedxor)
    cv2.imshow("maskednot", maskednot)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cv2.destroyAllWindows()
