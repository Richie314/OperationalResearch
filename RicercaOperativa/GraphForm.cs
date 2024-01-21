using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RicercaOperativa
{
    public partial class GraphForm : Form
    {
        public GraphForm()
        {
            InitializeComponent();
            graphViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            Microsoft.Msagl.Drawing.Graph graph = new("graph");
            //create the graph content 
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("A", "C").Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
            graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.MistyRose;
            Microsoft.Msagl.Drawing.Node c = graph.FindNode("C");
            c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            c.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;
            //bind the graph to the viewer 
            graphViewer.Graph = graph;
        }
        public Microsoft.Msagl.GraphViewerGdi.GViewer graphViewer;

        private void GraphForm_Load(object sender, EventArgs e)
        {
            GraphViewerInit();
        }
        void GraphViewerInit(string name = "Graph")
        {
            SuspendLayout();
            graphViewer.Name = string.IsNullOrWhiteSpace(name) ? "Graph" : name;
            /*
             * Width, height and controls
             */
            graphViewer.AutoScroll = true;
            graphViewer.Anchor = 
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;
            graphViewer.CurrentLayoutMethod =
                Microsoft.Msagl.GraphViewerGdi.LayoutMethod.IcrementalLayout;

            graphViewer.Location = new Point(0, 0);
            graphViewer.LooseOffsetForRouting = 0.25D;
            graphViewer.Margin = new Padding(0, 0, 0, 0);
            graphViewer.Padding = new Padding(3, 3, 3, 3);
            graphViewer.ClientSize = ClientSize;

            /*
             * Toolbar and its buttons
             */
            graphViewer.ToolBarIsVisible = true;
            graphViewer.PanButtonPressed = false;
            graphViewer.SaveAsImageEnabled = true;
            graphViewer.SaveAsMsaglEnabled = false;
            graphViewer.SaveButtonVisible = true;
            graphViewer.SaveGraphButtonVisible = true;
            graphViewer.SaveInVectorFormatEnabled = true;
            graphViewer.UndoRedoButtonsVisible = false;
            graphViewer.BackwardEnabled = false;
            graphViewer.ForwardEnabled = false;
            graphViewer.EdgeInsertButtonVisible = false;
            graphViewer.LayoutEditingEnabled = false;
            graphViewer.LayoutAlgorithmSettingsButtonVisible = false;

            /*
             * Background color
             */
            graphViewer.BackColor = Color.White;


            Controls.Add(graphViewer);
            ResumeLayout();
        }
    }
}
