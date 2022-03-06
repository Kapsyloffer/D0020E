import numpy

a = numpy.array([1, 2, 3, 4])
a = numpy.append(a, numpy.array([5, 6, 7, 8]))
a = numpy.append(a, numpy.array([9, 10, 11, 12]))
a = numpy.delete(a, [0, 1, 2, 3])

# a = numpy.reshape(a, (2, 4))

print(a)
# print(len(a))

b = numpy.array([1, 2.5151, 3, 4.26262, 5, 6, 7, 8])

for i in range(len(b)):
    if b[i] != int:
        x = int(b[i])
        b = numpy.append(b, x)

print(b[2])


