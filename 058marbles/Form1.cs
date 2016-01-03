﻿using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _058marbles
{
  public partial class Form1 : Form
  {
    static readonly string rev = "$Rev$".Split( ' ' )[ 1 ];

    /// <summary>
    /// Scene center point.
    /// </summary>
    protected Vector3 center = Vector3.Zero;

    /// <summary>
    /// Scene diameter.
    /// </summary>
    protected float diameter = 30.0f;

    /// <summary>
    /// GLControl guard flag.
    /// </summary>
    bool loaded = false;

    #region OpenGL globals

    public static uint[] VBOid = new uint[ 2 ];       // vertex array (colors, coords), index array

    #endregion

    #region FPS counter

    long lastFpsTime = 0L;
    int frameCounter = 0;
    static public long triangleCounter = 0L;
    double lastFps = 0.0;
    double lastTps = 0.0;

    #endregion

    public Form1 ()
    {
      InitializeComponent();
      Text += " (rev: " + rev + ')';
    }

    private void buttonInit_Click ( object sender, EventArgs e )
    {
      Cursor.Current = Cursors.WaitCursor;
      string param = textParam.Text;

      // TODO: stop the simulation thread?

      if ( world == null )
        world = new MarblesWorld();
      world.Init( param );
      data = null;

      // TODO: start the simulation thread?

      Cursor.Current = Cursors.Default;

      glControl1.Invalidate();
    }

    private void glControl1_Load ( object sender, EventArgs e )
    {
      loaded = true;

      // OpenGL init code:
      GL.ClearColor( Color.DarkBlue );
      GL.Enable( EnableCap.DepthTest );
      GL.ShadeModel( ShadingModel.Flat );

      // VBO init:
      GL.GenBuffers( 2, VBOid );
      if ( GL.GetError() != ErrorCode.NoError )
      {
        MessageBox.Show( "VBO extension not present!", "VBO Error" );
        Program.form.Close();
      }

      // Simulation scene and related stuff:
      world = new MarblesWorld();
      renderer = new MarblesRenderer();
      world.Init( textParam.Text );

      SetupViewport();

      Application.Idle += new EventHandler( Application_Idle );

      // TODO: start the simulation thread?
    }

    private void glControl1_Resize ( object sender, EventArgs e )
    {
      if ( !loaded ) return;

      SetupViewport();
      glControl1.Invalidate();
    }

    private void glControl1_Paint ( object sender, PaintEventArgs e )
    {
      Render();
    }

    private void control_PreviewKeyDown ( object sender, PreviewKeyDownEventArgs e )
    {
      if ( e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right )
        e.IsInputKey = true;
    }
  }
}
