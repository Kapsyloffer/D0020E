import cv2
import numpy
import packages.opencv.Drawtools as Drawtools
import packages.general.Functions as Functions


class TrackerO:
    def __init__(self, type):
        self.tag = None

        self.target = None
        self.output = None
        self.outputVal = None

        self.trackerType = type
        self.trackOk = None
        self.init = False

        self.trackBox = None
        self.rootBox = None

        # Attempt to handle tracker tracking black area
        # self.trackFrame = FrameO.FrameObjectSingle("trackFrame")

        # Attempt to combat track loss
        self.trackBoxArray = numpy.array([])
        self.extrapolatedBox = None
        self.extrapolationCount = 0
        self.extrapolationCountTotal = 0
        self.columns = 3
        self.rows = 4

    def setRootBox(self, array):
        self.rootBox = array

    def setTarget(self, inputObject):
        self.target = inputObject

    def setOutPut(self, frame):
        self.output = frame

    def clearTracker(self):
        self.tracker = None

    def clearTrackerFull(self):
        self.tracker = None
        self.init = False
        self.rootBox = None

    def clearTrackBox(self):
        self.trackBox = None

    def setTag(self, tag):
        self.tag = tag

    def setTrackerType(self, param):
        if param == "csrt":
            self.trackerType = "csrt"
        elif param == "boosting":
            self.trackerType = "boosting"

    def getTrackBox(self):
        return self.trackBox

    def getOutputVal(self):
        return self.outputVal

    def alterUpdate(self):
        self.rootBox = self.target.getMidPoint()
        if self.rootBox is None:
            return

        if not self.checkBoundary(self.output, self.rootBox):
            self.clearTrackerFull()
            return

        if not self.init:
            self.getNewTracker()
            self.trackBox = self.rootBox
            self.trackOk = self.tracker.init(self.output, tuple(self.trackBox))
            self.init = True

        self.trackOk, tempBox = self.tracker.update(self.output)
        self.trackBox = numpy.asarray(tempBox)

        if self.checkRoi(self.target.getFrame(), self.trackBox):
            self.clearTrackerFull()
            return

        if not self.trackOk:
            Drawtools.drawMultipleText(self.output,
                                       ("Tracking Failure", "Reset..."),
                                       numpy.array([100, 380]), (0, 0, 255), 20)
            self.clearTrackerFull()
            return

        # 3 set failure conditions , if else next draw
        if self.trackBox[0] < 0 or self.trackBox[1] < 0 or self.trackBox[0] > 499 or self.trackBox[1] > 499:
            Drawtools.drawMultipleText(self.output,
                                       ("Box out of bounds", "Attempting retrack..."),
                                       numpy.array([100, 380]), (0, 0, 255), 20)
            self.clearTrackerFull()
            return

        else:
            Drawtools.drawBox(self.output, self.trackBox, (0, 255, 0))
            Drawtools.drawMultipleText(self.output, ("trackBox",
                                                     "X: " + str(int(self.trackBox[0])),
                                                     "Xw: " + str(int(self.trackBox[0]) + int(self.trackBox[2])),
                                                     "Y: " + str(int(self.trackBox[1])),
                                                     "Yh: " + str(
                                                         int(self.trackBox[1]) + int(self.trackBox[3]))),
                                       numpy.array([100, 380]),
                                       (0, 255, 0), 20)
            self.outputVal = Functions.boxCordsToString(self.trackBox, "Seb")

    def getNewTracker(self):
        if self.trackerType == "csrt":
            self.tracker = cv2.TrackerCSRT_create()
        elif self.trackerType == "boosting":
            self.tracker = cv2.TrackerBoosting_create()
        elif self.trackerType == "mil":
            self.tracker = cv2.TrackerMIL_create()
        elif self.trackerType == "kcf":
            self.tracker = cv2.TrackerKCF_create()

    def checkRoi(self, frame, trackbox):
        trackbox = trackbox.astype(int)
        x, y, w, h = trackbox[0], trackbox[1], trackbox[0] + trackbox[2], trackbox[1] + trackbox[3]

        if self.checkBoundary(frame, trackbox):
            crop = frame[y:h, x:w]
            cv2.imshow("crop", crop)
            if numpy.mean(crop) < 8:
                return True
            return False

    def checkBoundary(self, frame, shape):
        dimensions = frame.shape
        height = dimensions[0]
        width = dimensions[1]
        midpoint = numpy.array([height // 2, width // 2])
        shape = shape.astype(int)
        x, y, w, h = shape[0], shape[1], shape[0] + shape[2], shape[1] + shape[3]
        condA = w < width and x > 0
        condB = h < height and y > 0

        if condA and condB:
            return True
        return False
