﻿using System.Globalization;
using System.Reflection;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Gulla.EpiserverAutomaticImageDescription.Core.Image.Attributes
{
    /// <summary>
    /// Analyze image for racy content. Apply to bool properties for true/false or double/string for racy score.
    /// </summary>
    public class AnalyzeImageForRacismAttribute : BaseImageDetailsAttribute
    {
        public override bool AnalyzeImageContent => true;

        public override void Update(object content, ImageAnalysis imageAnalyzerResult, OcrResult ocrResult, PropertyInfo propertyInfo)
        {
            if (imageAnalyzerResult.Adult == null)
            {
                return;
            }

            if (IsBooleanProperty(propertyInfo))
            {
                propertyInfo.SetValue(content, imageAnalyzerResult.Adult.IsRacyContent);
            }
            else if (IsDoubleProperty(propertyInfo))
            {
                propertyInfo.SetValue(content, imageAnalyzerResult.Adult.RacyScore);
            }
            else if (IsStringProperty(propertyInfo))
            {
                propertyInfo.SetValue(content, imageAnalyzerResult.Adult.RacyScore.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}