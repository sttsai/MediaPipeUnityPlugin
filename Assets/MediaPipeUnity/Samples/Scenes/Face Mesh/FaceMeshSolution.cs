// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe.Unity.Sample.FaceMesh
{
  public class FaceMeshSolution : ImageSourceSolution<FaceMeshGraph>
  {
    [SerializeField] private DetectionListAnnotationController _faceDetectionsAnnotationController;
    [SerializeField] private MultiFaceLandmarkListAnnotationController _multiFaceLandmarksAnnotationController;
    [SerializeField] private NormalizedRectListAnnotationController _faceRectsFromLandmarksAnnotationController;
    [SerializeField] private NormalizedRectListAnnotationController _faceRectsFromDetectionsAnnotationController;

    public int maxNumFaces
    {
      get => graphRunner.maxNumFaces;
      set => graphRunner.maxNumFaces = value;
    }

    public bool refineLandmarks
    {
      get => graphRunner.refineLandmarks;
      set => graphRunner.refineLandmarks = value;
    }

    public float minDetectionConfidence
    {
      get => graphRunner.minDetectionConfidence;
      set => graphRunner.minDetectionConfidence = value;
    }

    public float minTrackingConfidence
    {
      get => graphRunner.minTrackingConfidence;
      set => graphRunner.minTrackingConfidence = value;
    }

    protected override void OnStartRun()
    {
      if (!runningMode.IsSynchronous())
      {
        graphRunner.OnFaceDetectionsOutput += OnFaceDetectionsOutput;
        graphRunner.OnMultiFaceLandmarksOutput += OnMultiFaceLandmarksOutput;
        graphRunner.OnFaceRectsFromLandmarksOutput += OnFaceRectsFromLandmarksOutput;
        graphRunner.OnFaceRectsFromDetectionsOutput += OnFaceRectsFromDetectionsOutput;
        graphRunner.OnFaceClassificationsFromBlendShapesOutput += OnFaceClassificationsFromBlendShapesOutput;
      }

      var imageSource = ImageSourceProvider.ImageSource;
      SetupAnnotationController(_faceDetectionsAnnotationController, imageSource);
      SetupAnnotationController(_faceRectsFromLandmarksAnnotationController, imageSource);
      SetupAnnotationController(_multiFaceLandmarksAnnotationController, imageSource);
      SetupAnnotationController(_faceRectsFromDetectionsAnnotationController, imageSource);
    }

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
      graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
      List<Detection> faceDetections = null;
      List<NormalizedLandmarkList> multiFaceLandmarks = null;
      List<NormalizedRect> faceRectsFromLandmarks = null;
      List<NormalizedRect> faceRectsFromDetections = null;
      ClassificationList faceBlendShapes = null;

      if (runningMode == RunningMode.Sync)
      {
        var _ = graphRunner.TryGetNext(out faceDetections, out multiFaceLandmarks, out faceRectsFromLandmarks, out faceRectsFromDetections, out faceBlendShapes, true);
      }
      else if (runningMode == RunningMode.NonBlockingSync)
      {
        yield return new WaitUntil(() => graphRunner.TryGetNext(out faceDetections, out multiFaceLandmarks, out faceRectsFromLandmarks, out faceRectsFromDetections, out faceBlendShapes, false));
      }

      _faceDetectionsAnnotationController.DrawNow(faceDetections);
      _multiFaceLandmarksAnnotationController.DrawNow(multiFaceLandmarks);
      _faceRectsFromLandmarksAnnotationController.DrawNow(faceRectsFromLandmarks);
      _faceRectsFromDetectionsAnnotationController.DrawNow(faceRectsFromDetections);
      if (faceBlendShapes != null)
        Debug.Log($"Blendshapes count {faceBlendShapes.Classification.Count}");
    }

    private void OnFaceDetectionsOutput(object stream, OutputEventArgs<List<Detection>> eventArgs)
    {
      _faceDetectionsAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnMultiFaceLandmarksOutput(object stream, OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
    {
      _multiFaceLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnFaceRectsFromLandmarksOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
    {
      _faceRectsFromLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnFaceRectsFromDetectionsOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
    {
      _faceRectsFromDetectionsAnnotationController.DrawLater(eventArgs.value);
    }

    
    private void OnFaceClassificationsFromBlendShapesOutput(object stream, OutputEventArgs<ClassificationList> eventArgs)
    {
      //Debug.Log($"Blendshapes count {eventArgs.value?.Classification?.Count}");
      if (eventArgs.value != null)
        GetBlendShapeValues(eventArgs.value);
    }

    public float[] BlendShapeValues;
    private float blendShapeValueScale=100f;
    private void GetBlendShapeValues(ClassificationList faceBlendShapesList)
    {
      if (faceBlendShapesList == null)
      {
        BlendShapeValues = null;
        return;
      }

      int count = faceBlendShapesList.Classification.Count;

      if(BlendShapeValues==null || count!= BlendShapeValues.Length)
        BlendShapeValues = new float[count];
      for (int i = 0; i < count; i++)
      {
        Classification mark = faceBlendShapesList.Classification[i];
        BlendShapeValues[i] = mark.Score * blendShapeValueScale;
      }
    }


    void OnGUI()
    {
      OnGUI_BlendShapesValue();
    }

    private GUIStyle _fontStyle;
    private GUIStyle _barStyle;
    private GUIStyle _barStyle_gb;
    private void OnGUI_BlendShapesValue()
    {
      if (BlendShapeValues == null)
        return;
      var scale = 0.5f;
      var fieldWidth = 200f;
      var fieldHeight = 40;
      var colPos1 = 20;
      var colPos2 = 50;
      var colPos3 = 300;
      var rect = new UnityEngine.Rect(20, 0, 100, 40 * scale);
      if (_fontStyle == null)
      {
        _fontStyle = new GUIStyle();
        _fontStyle.fontSize = Mathf.RoundToInt(30 * scale);
        _fontStyle.normal.textColor = UnityEngine.Color.white;
        _barStyle = new GUIStyle();
        _barStyle.normal.background = Texture2D.whiteTexture;
        _barStyle_gb = new GUIStyle();
        _barStyle_gb.normal.background = Texture2D.grayTexture;
      }
      rect.y += fieldHeight;
      var values = BlendShapeValues;
      for (int i = 0; i < values.Length; i++)
      {
        float value = values[i];
        rect.x = colPos1;
        rect.width = fieldWidth;
        GUI.Label(rect, $"{i}", _fontStyle);
        rect.x = colPos2;
        GUI.Label(rect, value.ToString(), _fontStyle);
        rect.x = colPos3;
        GUI.Box(rect, "", _barStyle_gb);
        rect.width = fieldWidth * (value / 100f);
        GUI.Box(rect, "", _barStyle);
        rect.y += fieldHeight * scale;
      }
    }
  }
}
