import cv2


def drawMultipleText(frame, inputtext, startingcoordinates, color , spacing):
    for t in inputtext:
        drawText(frame, t, tuple(startingcoordinates), color)
        startingcoordinates[1] = startingcoordinates[1] + spacing
    return


def drawBox(frame, box, color):
    x, y, w, h = int(box[0]), int(box[1]), int(box[2]), int(box[3])
    cv2.rectangle(frame, (x, y), ((x + w), (y + h)), color, 3, 1)


def drawText(frame, text, coords, color):
    cv2.putText(frame, text, tuple(coords), cv2.FONT_HERSHEY_SIMPLEX, 0.75, color, 2)
