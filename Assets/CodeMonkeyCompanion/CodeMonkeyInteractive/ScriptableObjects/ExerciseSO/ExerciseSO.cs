using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeMonkey.CSharpCourse.Interactive {

    //[CreateAssetMenu()]
    public class ExerciseSO : ScriptableObject {


        public string exerciseNumber;
        public string exerciseName;
        public string exerciseTitle;
        public string videoWalkthroughUrl;

        [TextArea(5, 10)]
        public string exerciseText;

        [TextArea(5, 10)]
        public string hintText;

        [TextArea(5, 10)]
        public string solutionText;


        public void LoadExerciseScene() {
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            string exercisesFolderPath = GetLectureFolder() + GetExerciseNameWithCode();

            if (Directory.Exists(exercisesFolderPath)) {
                string[] fileArray = Directory.GetFiles(exercisesFolderPath);
                foreach (string fileName in fileArray) {
                    bool isScene = fileName.Contains(".unity");
                    bool isSceneMeta = fileName.Contains(".unity.meta");
                    if (isScene && !isSceneMeta) {
                        string filePath = fileName;
                        filePath = filePath.Substring(Application.dataPath.Length);
                        filePath = "Assets" + filePath;
                        EditorSceneManager.OpenScene(filePath);
                        EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(filePath));
                    }
                }
            }
        }

        public void StartStopCompleteExercise() {
            if (IsExerciseActive()) {
                CompleteExercise();
            } else {
                StartExercise();
            }
        }

        private string GetLectureFolder() {
            LectureSO lectureSO = LectureSO.GetLectureSO(this);
            string lectureFolder = $"L{lectureSO.lectureCode}00_{lectureSO.lectureName}";
            string exercisesFolderPath = Application.dataPath +
                $"/Lectures/{lectureFolder}/";

            return exercisesFolderPath;
        }

        private string GetExerciseNameWithCode() {
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            return $"L{lectureSO.lectureCode}{exerciseNumber}_{exerciseName}";
        }
        
        private string GetZipPath() {
            string exercisesFolderPath = GetLectureFolder();
            string zipPath = exercisesFolderPath + GetExerciseNameWithCode() + ".zip";
            return zipPath;
        }

        private void StartExercise() {
            Debug.Log("Starting Exercise: " + exerciseNumber + " " + exerciseName);
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            string exercisesFolderPath = GetLectureFolder();
            string zipPath = GetZipPath();

            List<string> unpackedFilePathList = new List<string>();

            using (ZipArchive zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Read)) {
                foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries) {
                    //string filePath = exercisesFolderPath + GetExerciseNameWithCode() + "/" + zipArchiveEntry.Name;
                    string filePath = exercisesFolderPath + zipArchiveEntry.FullName;
                    if (zipArchiveEntry.Length == 0) {
                        // Folder
                        Directory.CreateDirectory(filePath);
                    } else {
                        zipArchiveEntry.ExtractToFile(filePath, true);
                        unpackedFilePathList.Add(filePath);
                    }
                }
            }

            CodeMonkeyInteractiveSO.SetActiveExerciseSO(this);
            CodeMonkeyInteractiveSO.SetState(this, CodeMonkeyInteractiveSO.State.Started);
            CodeMonkeyInteractiveSO.ClearObjectiveCheckboxState();

            AssetDatabase.Refresh();
            CompilationPipeline.RequestScriptCompilation();

            LoadExerciseScene();
        }

        public void StopExercise() {
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            if (EditorApplication.isPlaying) {
                Debug.LogWarning("Stop Playing in Unity in order to Complete the exercise!");
                EditorApplication.isPlaying = false;
                return;
            }

            CodeMonkeyInteractiveSO.LoadDefaultScene();

            string exercisesFolderPath = GetLectureFolder() + GetExerciseNameWithCode();

            if (Directory.Exists(exercisesFolderPath)) {
                Directory.Delete(exercisesFolderPath, true);
                File.Delete(exercisesFolderPath + ".meta");
            }

            CodeMonkeyInteractiveSO.ClearActiveExerciseSO();
            CodeMonkeyInteractiveSO.SetState(this, CodeMonkeyInteractiveSO.State.None);

            AssetDatabase.Refresh();
        }

        public void CompleteExercise() {
            if (EditorApplication.isPlaying) {
                Debug.LogWarning("Stop Playing in Unity in order to Complete the exercise!");
                EditorApplication.isPlaying = false;
                return;
            }

            Debug.Log("Completed Exercise: " + exerciseNumber + " " + exerciseName);
            StopExercise();
            CodeMonkeyInteractiveSO.SetState(this, CodeMonkeyInteractiveSO.State.Completed);
            AssetDatabase.Refresh();
        }

        public void SetCompleted() {
            StopExercise();
            CodeMonkeyInteractiveSO.SetState(this, CodeMonkeyInteractiveSO.State.Completed);
            AssetDatabase.Refresh();
        }

        public void SetUnCompleted() {
            StopExercise();
            CodeMonkeyInteractiveSO.SetState(this, CodeMonkeyInteractiveSO.State.None);
            AssetDatabase.Refresh();
        }

        public bool IsCompleted() {
            return CodeMonkeyInteractiveSO.GetState(this) == CodeMonkeyInteractiveSO.State.Completed;
        }

        public bool IsExerciseActive() {
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            string exercisesFolderPath = GetLectureFolder() + GetExerciseNameWithCode();

            if (Directory.Exists(exercisesFolderPath)) {
                // This exercise is active!
                return true;
            }

            // Exercise not active
            return false;
        }


        public static bool IsAnyExerciseActive() {
            LectureListSO lectureListSO = LectureListSO.GetLectureListSO();
            foreach (LectureSO lectureSO in lectureListSO.lectureSOList) {
                if (lectureSO.exerciseListSO.exerciseSOList.Count > 0) {
                    // This lecture has exercises, check if any are active
                    string lectureFolder = $"{lectureSO.lectureCode}_{lectureSO.lectureName}";
                    string exercisesFolderPath = Application.dataPath +
                        $"/Lectures/{lectureFolder}/Exercises/";

                    if (Directory.Exists(exercisesFolderPath)) {
                        string[] fileArray = Directory.GetFiles(exercisesFolderPath, "*.cs");
                        if (fileArray.Length > 0) {
                            // This exercise is active!
                            return true;
                        }
                    }
                }
            }
            // No exercises are active
            return false;
        }

    }

}