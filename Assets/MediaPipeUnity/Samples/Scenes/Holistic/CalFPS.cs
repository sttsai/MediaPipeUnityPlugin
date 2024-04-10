using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XR.BodyTracking
{
    public class CalFPS
    { 
        public struct DeltaTime
        {
            public float timeStamp;
            public float deltaTime;
            public float fps;
        };

        private DeltaTime renderFps; 
        private DeltaTime poseFps;
        private DeltaTime poseWorldFps;
        private DeltaTime leftHandFps;
        private DeltaTime rightHandFps;
        private DeltaTime faceFps;

        public CalFPS()
        {
        }

        public DeltaTime PoseWorldFps
        {
            get => poseWorldFps;
        }

        public string GetFPSInfo()
        {
          return string.Format("FPS:{0:000.} P:{1:00.} PW:{2:00.} LH:{3:00.} RH:{4:00.}", renderFps.fps, poseFps.fps, poseWorldFps.fps, leftHandFps.fps, rightHandFps.fps);
        }

        public static void UpdateFPS(ref DeltaTime target)
        {
            float nowTime = TimeSystem.NowTime;
            float timeDiff = nowTime - target.timeStamp;
            target.timeStamp = nowTime;

            target.deltaTime += (timeDiff - target.deltaTime) * 0.1f;
            target.fps = 1.0f / target.deltaTime;
        }

        public void UpdateRenderTimer()
        {
            UpdateFPS(ref renderFps);
            TimeSystem.RenderFPS = renderFps.fps;
        }

        public void UpdatePoseTimer()
        {
            UpdateFPS(ref poseFps);
        }

        public void UpdatePoseWorldTimer()
        {
            UpdateFPS(ref poseWorldFps);
            TimeSystem.PoseWorldFPS = poseWorldFps.fps;
        }

        public void UpdateLeftHandTimer()
        {
            UpdateFPS(ref leftHandFps);
            TimeSystem.LeftHandFPS = leftHandFps.fps;
        }

        public void UpdateRightHandTimer()
        {
            UpdateFPS(ref rightHandFps);
            TimeSystem.RightHandFPS = rightHandFps.fps;
        }

        public void UpdateFaceTimer()
        {
            UpdateFPS(ref faceFps);
        }

    }
}
