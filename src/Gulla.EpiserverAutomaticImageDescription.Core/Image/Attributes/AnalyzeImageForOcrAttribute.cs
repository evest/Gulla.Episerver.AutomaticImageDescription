﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gulla.EpiserverAutomaticImageDescription.Core.Translation;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Gulla.EpiserverAutomaticImageDescription.Core.Image.Attributes
{
    /// <summary>
    /// Analyze image and perform Optical Character Recognition (OCR). Apply to string properties.
    /// </summary>
    public class AnalyzeImageForOcrAttribute : BaseImageDetailsAttribute
    {
        public AnalyzeImageForOcrAttribute()
        {
        }

        /// <summary>
        /// Analyze image and perform Optical Character Recognition (OCR). Apply to string properties.
        /// </summary>
        /// <param name="toLanguage">Translate OCR result to specified language.</param>
        /// <param name="fromLanguage">If you know what language the source text is, specify. If null, the algorithm will try to detect source language.</param>
        public AnalyzeImageForOcrAttribute(string toLanguage, string fromLanguage = null)
        {
            FromLanguageCode = fromLanguage;
            ToLanguageCode = toLanguage;
        }

        private string FromLanguageCode { get; }
        private string ToLanguageCode { get; }

        public override bool AnalyzeImageOcr => true;

        public override void Update(object content, ImageAnalysis imageAnalyzerResult, OcrResult ocrResult, PropertyInfo propertyInfo)
        {
            if (ocrResult.Regions == null || ocrResult.Regions.Count == 0)
            {
                return;
            }

            var ocrTranslated = GetTranslatedOcr(ocrResult, propertyInfo);

            if (IsStringProperty(propertyInfo))
            {
                propertyInfo.SetValue(content, string.Join(" ", ocrTranslated));
            }
        }

        private static IEnumerable<string> GetTranslatedOcr(OcrResult ocrResult, PropertyInfo propertyInfo)
        {
            var words = ocrResult.Regions.Select(x => x.Lines).SelectMany(x => x).Select(x => x.Words).SelectMany(x => x).Select(x => x.Text);
            var toLanguage = propertyInfo.GetCustomAttribute<AnalyzeImageForOcrAttribute>().ToLanguageCode;
            if (toLanguage == null)
            {
                return words;
            }

            var fromLanguage = propertyInfo.GetCustomAttribute<AnalyzeImageForOcrAttribute>().FromLanguageCode;
            return Translator.TranslateText(words, toLanguage, fromLanguage).Select(x => x.Translations).Select(x => x.First().Text);
        }
    }
}