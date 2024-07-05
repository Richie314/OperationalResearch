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
using OperationalResearch.Models;
using OperationalResearch.Models.Graphs;

namespace OperationalResearch.ViewForms
{
    public partial class GraphForm<GraphType, GraphEdge> : Form
        where GraphType : Graph<GraphEdge>
        where GraphEdge : Edge
    {
        private readonly GraphType GraphToShow;
        public GraphForm(GraphType graphToDisplay)
        {
            InitializeComponent();
            GraphToShow = graphToDisplay;

            graphViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            // create a graph object 
            Microsoft.Msagl.Drawing.Graph graph = new("graph");

            // create the graph content 
            foreach (GraphEdge Edge in graphToDisplay.Edges)
            {
                var e = graph.AddEdge(
                    (Edge.From + 1).ToString(), Edge.Label, (Edge.To + 1).ToString());
                e.Attr.LineWidth = 1;
                e.Attr.Weight = 1;
                e.Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
            }
            foreach (var Node in graph.Nodes)
            {
                if (Node.Id == "0")
                {
                    Node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
                    Node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Diamond;
                } else
                {
                    Node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.AntiqueWhite;
                }
            }
            
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
