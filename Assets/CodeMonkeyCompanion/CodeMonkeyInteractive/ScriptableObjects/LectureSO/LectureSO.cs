using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.CSharpCourse.Interactive {

    //[CreateAssetMenu()]
    public class LectureSO : ScriptableObject {


        public int lectureCode;
        public string lectureNumber;
        public string lectureName;
        public string lectureTitle;
        public ExerciseListSO exerciseListSO;

        [TextArea(10, 20)]
        public string lectureDescription;



        public string GetLectureFolderPath() {
            return Application.dataPath + $"/Lectures/{lectureCode}_{lectureName}/";
        }


        public class LectureStats {
            public int exercisesDone;
            public int exercisesTotal;
        }

        public LectureStats GetLectureStats() {
            GetLectureStats(
                out int exercisesDone,
                out int exercisesTotal);

            LectureStats lectureStats = new LectureStats();
            lectureStats.exercisesDone = exercisesDone;
            lectureStats.exercisesTotal = exercisesTotal;
            return lectureStats;
        }

        public void GetLectureStats(
            out int exercisesDone,
            out int exercisesTotal) 
        {
            exercisesDone = 0;
            exercisesTotal = exerciseListSO.exerciseSOList.Count;
            foreach (ExerciseSO exerciseSO in exerciseListSO.exerciseSOList) {
                if (CodeMonkeyInteractiveSO.GetState(exerciseSO) == CodeMonkeyInteractiveSO.State.Completed) {
                    exercisesDone++;
                }
            }
        }



        public static LectureSO GetLectureSO(string partialLectureSOName) {
            LectureListSO lectureListSO = LectureListSO.GetLectureListSO();

            foreach (LectureSO lectureSO in lectureListSO.lectureSOList) {
                if (lectureSO.name.Contains(partialLectureSOName)) {
                    return lectureSO;
                }
            }

            return null;
        }

        public static LectureSO GetLectureSO(ExerciseSO forExerciseSO) {
            LectureListSO lectureListSO = LectureListSO.GetLectureListSO();

            foreach (LectureSO lectureSO in lectureListSO.lectureSOList) {
                foreach (ExerciseSO exerciseSO in lectureSO.exerciseListSO.exerciseSOList) {
                    if (exerciseSO == forExerciseSO) {
                        return lectureSO;
                    }
                }
            }

            return null;
        }

    }

}