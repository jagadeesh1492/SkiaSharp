﻿using System;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using SKFormsView = SkiaSharp.Views.Forms.SKView;
using SKNativeView = SkiaSharp.Views.SKView;

[assembly: ExportRenderer(typeof(SKFormsView), typeof(SkiaSharp.Views.Forms.SKViewRenderer))]

namespace SkiaSharp.Views.Forms
{
	internal class SKViewRenderer : ViewRenderer<SKFormsView, SKNativeView>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<SKFormsView> e)
		{
			if (e.OldElement != null)
			{
				var oldController = (ISKViewController)e.OldElement;

				// unsubscribe from events
				oldController.SurfaceInvalidated -= OnSurfaceInvalidated;
			}

			if (e.NewElement != null)
			{
				var newController = (ISKViewController)e.NewElement;

				// create the native view
				var view = new InternalView(Context, newController);
				SetNativeControl(view);

				// subscribe to events from the user
				newController.SurfaceInvalidated += OnSurfaceInvalidated;

				// paint for the first time
				Control.Invalidate();
			}

			base.OnElementChanged(e);
		}

		protected override void Dispose(bool disposing)
		{
			// detach all events before disposing
			var controller = (ISKViewController)Element;
			if (controller != null)
			{
				controller.SurfaceInvalidated -= OnSurfaceInvalidated;
			}

			base.Dispose(disposing);
		}

		private void OnSurfaceInvalidated(object sender, EventArgs eventArgs)
		{
			// repaint the native control
			Control.Invalidate();
		}

		private class InternalView : SKNativeView
		{
			private readonly ISKViewController controller;

			public InternalView(Context context, ISKViewController controller)
				: base(context)
			{
				this.controller = controller;
			}

			protected override void OnDraw(SKSurface surface, SKImageInfo info)
			{
				base.OnDraw(surface, info);

				// the control is being repainted, let the user know
				controller.OnPaintSurface(new SKPaintSurfaceEventArgs(surface, info));
			}
		}
	}
}