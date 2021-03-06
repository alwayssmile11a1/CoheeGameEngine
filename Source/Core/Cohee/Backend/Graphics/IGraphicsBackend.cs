﻿using Cohee.Drawing;
using System;
using System.Collections.Generic;

namespace Cohee.Backend
{
    public interface IGraphicsBackend : ICoheeBackend
    {
        IEnumerable<ScreenResolution> AvailableScreenResolutions { get; }
        Point2 ExternalBackbufferSize { get; set; }

        void BeginRendering(IDrawDevice device, RenderOptions options, RenderStats stats = null);
        void Render(IReadOnlyList<DrawBatch> batches);
        void EndRendering();

        INativeGraphicsBuffer CreateBuffer(GraphicsBufferType type);
        INativeTexture CreateTexture();
        INativeRenderTarget CreateRenderTarget();
        INativeShaderPart CreateShaderPart();
        INativeShaderProgram CreateShaderProgram();
        INativeWindow CreateWindow(WindowOptions options);

        /// <summary>
        /// Retrieves the main rendering buffer's pixel data from video memory in the Rgba8 format.
        /// 
        /// Note that generic, array-based variants of this method are available via extension method
        /// when using the Cohee.Backend namespace.
        /// </summary>
        /// <param name="target">The target buffer to store transferred pixel data in.</param>
        /// <param name="dataLayout">The desired color layout of the specified buffer.</param>
        /// <param name="dataElementType">The desired color element type of the specified buffer.</param>
        /// <param name="x">The x position of the rectangular area to read.</param>
        /// <param name="y">The y position of the rectangular area to read.</param>
        /// <param name="width">The width of the rectangular area to read. Defaults to the rendering targets width.</param>
        /// <param name="height">The height of the rectangular area to read. Defaults to the rendering targets height.</param>
        void GetOutputPixelData(IntPtr target, ColorDataLayout dataLayout, ColorDataElementType dataElementType, int x, int y, int width, int height);
    }
}
