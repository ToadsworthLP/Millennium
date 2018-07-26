using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SavePort.Types {

    [Serializable]
    [CreateAssetMenu(fileName = "New DateTime Container", menuName = "Container Assets/DateTime")]
    public class DateTimeContainer : BaseDataContainer<DateTime> {

        public override DateTime Validate(DateTime input) {
            return input;
        }

#if UNITY_EDITOR
        private int year;
        private int month;
        private int day;
        private int seconds;
        private int minutes;
        private int hours;

        protected override void OnContainerInspectorGUI() {
            base.OnContainerInspectorGUI();

            EditorGUILayout.LabelField("Editor Value: " + editorValue.ToString());

            EditorGUILayout.BeginHorizontal();

            year = EditorGUILayout.IntField(year);
            if (year > 9999 || year <= 0) { year = editorValue.Year; }
            GUILayout.Label("y ");

            month = EditorGUILayout.IntField(month);
            if (month > 12 || month <= 0) { month = editorValue.Month; }
            GUILayout.Label("m ");

            day = EditorGUILayout.IntField(day);
            if (day > DateTime.DaysInMonth(year, month) || day <= 0) { day = editorValue.Day; }
            GUILayout.Label("d ");

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            hours = EditorGUILayout.IntField(hours);
            if (hours >= 24 || hours < 0) { hours = editorValue.Hour; }
            GUILayout.Label("hr ");

            minutes = EditorGUILayout.IntField(minutes);
            if (minutes >= 60 || minutes < 0) { minutes = editorValue.Minute; }
            GUILayout.Label("min ");

            seconds = EditorGUILayout.IntField(seconds);
            if (seconds >= 60 || seconds < 0) { seconds = editorValue.Second; }
            GUILayout.Label("sec ");

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Set Time")) {
                editorValue = new DateTime(year, month, day, hours, minutes, seconds);
                OnValidate();
            }

            if (GUILayout.Button("Now")) {
                year = editorValue.Year;
                month = editorValue.Month;
                day = editorValue.Day;
                hours = editorValue.Hour;
                minutes = editorValue.Minute;
                seconds = editorValue.Second;

                editorValue = DateTime.Now;
                OnValidate();
            }

            EditorGUILayout.EndHorizontal();
        }

        protected override bool HasCustomInspector() {
            return true;
        }
#endif
    }

    [Serializable]
    public class DateTimeReference : BaseDataReference<DateTime, DateTimeContainer> { }

}