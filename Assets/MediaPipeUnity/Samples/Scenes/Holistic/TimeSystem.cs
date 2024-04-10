using System;
using UnityEngine;

namespace XR.BodyTracking
{
    // 時間系統:開啟到目前的時間累積
    public static class TimeSystem {
        private static readonly long StartTicks = DateTime.Now.Ticks;
        public static float NowTime => (float)((DateTime.Now.Ticks - StartTicks) / (double)TimeSpan.TicksPerSecond);


        // render 
        private static float _renderfps=0.0f;
        //private static OneEuroFilter1 _renderfps_filter = new OneEuroFilter1();
        public static float RenderFPS
        {
            get => _renderfps;
            set
            {
                _renderfps = value;
            }
        }

        // pose world
        private static float _poseWorldfps=30.0f;
        //private static OneEuroFilter1 _poseWorldfps_filter = new OneEuroFilter1();
        public static float PoseWorldFPS
        {
            get => _poseWorldfps;
            set
            {
                _poseWorldfps =  value;
                _poseWorldDeltaTime = 1.0f / _poseWorldfps;
            } 
        }

        private static float _poseWorldDeltaTime= 0.033f;

        public static float PoseWorldDeltaTime
        {
            get => _poseWorldDeltaTime;
        }

        // left hand
        private static float _leftHandfps = 30.0f;
        //private static OneEuroFilter1 _leftHandfps_filter = new OneEuroFilter1();
        public static float LeftHandFPS
        {
            get => _leftHandfps;
            set
            {
                _leftHandfps = value;
                _leftHandDeltaTime = 1.0f / _leftHandfps;
            }
        }

        private static float _leftHandDeltaTime = 0.033f;

        public static float LeftHandDeltaTime
        {
            get => _leftHandDeltaTime;
        }

        // right hand
        private static float _rightHandfps = 30.0f;
        //private static OneEuroFilter1 _rightHandfps_filter = new OneEuroFilter1();
        public static float RightHandFPS
        {
            get => _rightHandfps;
            set
            {
                _rightHandfps = value;
                _rightHandDeltaTime = 1.0f / _leftHandfps;
            }
        }

        private static float _rightHandDeltaTime = 0.033f;

        public static float RightHandDeltaTime
        {
            get => _rightHandDeltaTime;
        }

    }
}
