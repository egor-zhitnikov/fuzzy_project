import numpy as np
import matplotlib.pyplot as plt
import std_mfs


class Word:

    def __init__(self, title, x, lmf, umf, base_on='std'):
        if base_on == 'custom':
            self.title = title
            self.U = x[:]
            self.lmf = lmf[:]
            self.umf = umf[:]
        elif base_on == 'std':
            self.title = title
            self.U = np.arange(*x)
            lmf_type, *lmf_params = lmf
            umf_type, *umf_params = umf
            self.range = (min(umf_params), max(umf_params))
            self.lmf = getattr(std_mfs, lmf_type)(self.U, *lmf_params)
            self.umf = getattr(std_mfs, umf_type)(self.U, *umf_params)

    def plot(self):
        fig, ax = plt.subplots()
        ax.set_ylabel(r'$\mu$(x)')
        ax.set_xlabel('x')
        ax.set_ylim([0, 1.2])
        ax.fill_between(self.U, self.lmf, self.umf, color="lightblue", edgecolor="blue", alpha=.9)
        plt.grid(True)
        plt.title(self.title)
        plt.show()

    def __str__(self):
        return self.title

    def __len__(self):
        return len(self.U)

    def similarity_measure(self, other):
        min_lmf = (min(a, b) for a, b in zip(self.lmf, other.lmf))
        max_lmf = (max(a, b) for a, b in zip(self.lmf, other.lmf))
        min_umf = (min(a, b) for a, b in zip(self.umf, other.umf))
        max_umf = (max(a, b) for a, b in zip(self.umf, other.umf))

        return (sum(min_umf) + sum(min_lmf)) / (sum(max_umf) + sum(max_lmf))