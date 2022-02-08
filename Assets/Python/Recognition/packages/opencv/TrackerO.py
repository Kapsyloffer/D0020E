import cv2


class TrackerO:
    def __init__(self):
        self.tracker = None
        self.trackers = []
        self.init = False
        self.trackOk = None
        self.trackBox = None

    def setTrackBox(self, array):
        self.trackBox = array

    def getTrackBox(self):
        return self.trackBox

    def crateTLD(self):
        self.trackers.append(cv2.TrackerTLD_create())

    def createMOSSE(self):
        self.trackers.append(cv2.TrackerMOSSE_create())

    def createCSRT(self):
        self.trackers.append(cv2.TrackerCSRT_create())

    def getTracker(self):
        return self.trackers[-1]

    def removeFirstTracker(self):
        self.trackers.remove(self.trackers[0])
