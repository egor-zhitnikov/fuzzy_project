import numpy as np


def __trapmf(x, a, b, c, d, h):
    if a <= x < b:
        return min((x - a) / (b - a), h)
    elif b <= x <= c:
        return min(1., h)
    elif c < x <= d:
        return min((d - x) / (d - c), h)
    return 0.


def trapmf(x, a: float, b: float, c: float, d: float, h=1):
    iterable = (__trapmf(item, a, b, c, d, h) for item in x)
    mf = np.fromiter(iterable, float)
    return np.nan_to_num(mf)


def __trimf(x, a, b, c, h):
    if a <= x < b:
        return min((x - a) / (b - a), h)
    elif x == b:
        return min(1., h)
    elif b < x <= c:
        return min((c - x) / (c - b), h)
    return 0.


def trimf(x, a: float, b: float, c: float, h=1):
    iterable = (__trimf(item, a, b, c, h) for item in x)
    mf = np.fromiter(iterable, float)
    return np.nan_to_num(mf)
