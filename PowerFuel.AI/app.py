import cv2
import numpy as np
import mediapipe as mp
import time

from fastapi import FastAPI, File, UploadFile, Form
from fastapi.responses import JSONResponse
from fastapi.middleware.cors import CORSMiddleware
from enum import Enum

from mediapipe.tasks import python
from mediapipe.tasks.python import vision


# =========================
# 1. APP (مرة واحدة فقط)
# =========================
app = FastAPI(title="AI Gym Coach")


from fastapi.responses import HTMLResponse

@app.get("/", response_class=HTMLResponse)
def home():
    return """
    <html>
        <head>
            <title>AI Gym Coach</title>
        </head>
        <body>
            <h2>AI Gym Coach Backend Running 🚀</h2>
            <p>Use /docs or frontend index.html</p>
        </body>
    </html>
    """
# =========================
# 2. CORS (قبل أي routes)
# =========================
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


# =========================
# 3. Exercises Enum
# =========================
class ExerciseName(str, Enum):
    squat = "squat"
    pushup = "pushup"
    plank = "plank"
    bicep_curl = "bicep_curl"


# =========================
# 4. AI Engine
# =========================
class GymAIAnalyzer:
    def __init__(self):
        base_options = python.BaseOptions(model_asset_path='pose_landmarker_heavy.task')
        options = vision.PoseLandmarkerOptions(
            base_options=base_options,
            output_segmentation_masks=False,
            running_mode=vision.RunningMode.IMAGE
        )
        self.detector = vision.PoseLandmarker.create_from_options(options)

        self.counter = 0
        self.stage = None
        self.start_time = None

    def calculate_angle(self, a, b, c):
        a = np.array([a.x, a.y])
        b = np.array([b.x, b.y])
        c = np.array([c.x, c.y])

        radians = np.arctan2(c[1]-b[1], c[0]-b[0]) - np.arctan2(a[1]-b[1], a[0]-b[0])
        angle = np.abs(radians * 180.0 / np.pi)
        if angle > 180:
            angle = 360 - angle
        return angle

    def analyze(self, frame, exercise_type):
        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb_frame)
        result = self.detector.detect(mp_image)

        if not result.pose_landmarks:
            return {"status": "error", "message": "لم يتم اكتشاف جسم"}

        landmarks = result.pose_landmarks[0]
        feedback = ""
        angle = 0
        timer_seconds = 0

        # ---- PLANK ----
        if exercise_type == ExerciseName.plank:
            angle = self.calculate_angle(landmarks[12], landmarks[24], landmarks[26])

            if 170 <= angle <= 190:
                if self.start_time is None:
                    self.start_time = time.time()
                timer_seconds = int(time.time() - self.start_time)
                feedback = f"ثبات ممتاز! {timer_seconds} ثانية"
            else:
                self.start_time = None
                feedback = "ظبط جسمك عشان يبدأ التايمر"

        # ---- SQUAT ----
        elif exercise_type == ExerciseName.squat:
            angle = self.calculate_angle(landmarks[24], landmarks[26], landmarks[28])

            if angle > 160:
                self.stage = "up"
            if angle < 100 and self.stage == "up":
                self.stage = "down"
                self.counter += 1

            feedback = "انزل أكثر" if self.stage == "up" else "كويس!"

        # ---- PUSHUP ----
        elif exercise_type == ExerciseName.pushup:
            angle = self.calculate_angle(landmarks[12], landmarks[14], landmarks[16])

            if angle > 160:
                self.stage = "up"
            if angle < 90 and self.stage == "up":
                self.stage = "down"
                self.counter += 1

            feedback = "انزل" if self.stage == "up" else "ادفع فوق"

        # ---- BICEP ----
        elif exercise_type == ExerciseName.bicep_curl:
            angle = self.calculate_angle(landmarks[12], landmarks[14], landmarks[16])

            if angle > 160:
                self.stage = "up"
            if angle < 40 and self.stage == "up":
                self.stage = "down"
                self.counter += 1

            feedback = "اثني الذراع" if self.stage == "up" else "صح!"

        return {
            "status": "success",
            "exercise": exercise_type,
            "angle": round(angle, 2),
            "reps_count": self.counter,
            "hold_timer": timer_seconds if exercise_type == ExerciseName.plank else None,
            "feedback": feedback
        }


# =========================
# 5. Analyzer instance
# =========================
analyzer = GymAIAnalyzer()


# =========================
# 6. API Route (MATCH frontend exactly)
# =========================
@app.post("/process_exercise")
async def process_exercise(
    exercise: ExerciseName = Form(...),
    file: UploadFile = File(...)
):
    contents = await file.read()
    nparr = np.frombuffer(contents, np.uint8)
    frame = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

    if frame is None:
        return JSONResponse(status_code=400, content={"message": "Invalid image"})

    result = analyzer.analyze(frame, exercise)
    return result


# =========================
# 7. Run server
# =========================
if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)