import numpy as np
import word

def alpha_cuts_intervals(m, h=1):
    intervals = [j / (m - 1) for j in range(m) if h >= j / (m - 1)]
    return intervals


def alpha_cut_lmf(y_value, codebook):
    res = []
    for item in codebook['words'].values():
        type_mf, a, b, c, d, h = item['lmf']
        x_alpha = ((b - a) * y_value + a, d - (d - c) * y_value)
        res.append(x_alpha)
    return res


def alpha_cut_umf(y_value, codebook):
    res = []
    for item in codebook['words'].values():
        type_mf, a, b, c, d = item['umf']
        x_alpha = ((b - a) * y_value + a, d - (d - c) * y_value)
        res.append(x_alpha)
    return res


def y_umf(intervals, codebook, W):
    umf_res = []

    for y_value in intervals:
        x_ranges = alpha_cut_umf(y_value, codebook)
        x_left = sum(x[0] * w for x, w in zip(x_ranges, W)) / sum(W)
        x_right = sum(x[1] * w for x, w in zip(x_ranges, W)) / sum(W)

        umf_res.append((x_left, x_right))

    return umf_res


def y_lmf(intervals, codebook, W):
    lmf_res = []

    for y_value in intervals:
        x_ranges = alpha_cut_lmf(y_value, codebook)
        x_left = sum(x[0] * w for x, w in zip(x_ranges, W)) / sum(W)
        x_right = sum(x[1] * w for x, w in zip(x_ranges, W)) / sum(W)

        lmf_res.append((x_left, x_right))

    return lmf_res


def constract_dit2fs(x, intervals_lmf, y_lmf, intervals_umf, y_umf):
    umf = np.zeros(len(x))
    lmf = np.zeros(len(x))

    for index, y in enumerate(intervals_umf):
        x_left, x_right = y_umf[index]
        left_index = np.argmax(x >= x_left)
        right_index = np.argmax(x >= x_right)
        for i in range(left_index, right_index + 1):
            umf[i] = y

    for index, y in enumerate(intervals_lmf):
        x_left, x_right = y_lmf[index]
        left_index = np.argmax(x >= x_left)
        right_index = np.argmax(x >= x_right)
        for i in range(left_index, right_index + 1):
            lmf[i] = y

    return word.Word(None, x, lmf, umf, base_on='custom')










