import words_model
import random
import lwa
import numpy as np
import word
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
app = FastAPI()

class FeedbackList(BaseModel):
    feedback: List[str]

@app.post("/calculate-feedback/")
async def calculate_feedback(feedback: FeedbackList):
    codebook = words_model.words_7
    W = []
    for item in codebook['words'].keys():
        W.append(feedback.feedback.count(item))

    print(feedback.feedback)
    print(W)

    h = min(item['lmf'][-1] for item in codebook['words'].values())
    m = 50
    intervals_umf = lwa.alpha_cuts_intervals(m)
    intervals_lmf = lwa.alpha_cuts_intervals(m, h)


    res_y_umf = lwa.y_umf(intervals_umf, codebook, W)
    res_y_lmf = lwa.y_lmf(intervals_lmf, codebook, W)

    res_word = lwa.constract_dit2fs(np.arange(*codebook['x']), intervals_lmf, res_y_lmf, intervals_umf, res_y_umf)
    #res_word.plot()

    sm = []
    for title, fou in codebook['words'].items():
        sm.append((
            title,
            res_word.similarity_measure(word.Word(None, codebook['x'], fou['lmf'], fou['umf']))
        ))

    return max(sm, key=lambda item: item[1])