using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EyeshotAnimation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Stopwatch Stopwatch = Stopwatch.StartNew();
        private static long GetCurrentTime() => Stopwatch.ElapsedMilliseconds;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private static Point2D min;
        private static Point2D max;
        private static Point2D center = new Point2D(114411, 86122);

        protected override void OnContentRendered(EventArgs e)
        {
            var fileName = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), @"Eyeshot Professional 2020 Samples\dataset\Assets\Misc\app8.dwg");            
            
            EyeshotModel.Layers.Add("DWG");
            var ra = new ReadAutodesk(fileName);
            ra.DoWork();
            ra.AddToSceneAsSingleObject(EyeshotModel, "DWG", "DWG");

                        

            min = Point2D.Origin;
            max = new Point2D(217478, 137969);
            
            for(int index = 0; index < 100; index++)
            {
                CreateVehicle(index);
            }            
            EyeshotModel.StartAnimation(25);
            EyeshotModel.ZoomFit();
            EyeshotModel.Invalidate();
            base.OnContentRendered(e);

            

        }

        private void CreateVehicle(int id)
        {
            var vehicleName = $"Vehicle-{id}";
            var vehicle = new Block(vehicleName);
            var vehicleCenter = new Circle(Plane.XY, Point2D.Origin, 1600);            
            vehicle.Entities.Add(vehicleCenter);
            var color = RandomColor.Next();
            foreach (var ent in vehicle.Entities)
            {                
                ent.ColorMethod = colorMethodType.byEntity;
                ent.Color = color;
            }

            EyeshotModel.Blocks.Add(vehicle);
            EyeshotModel.Entities.Add(new Translating(vehicleName));
        }

        class Translating : BlockReference
        {
            public static Random Random = new Random(System.Environment.TickCount);

            private long lastTime = GetCurrentTime();

            Vector2D direction;
            Point2D pos = center;
            
            public Translating(string blockName)
                : base(0, 0, 0, blockName, 1, 1, 1, 0)
            {
                direction = GetDirection();
            }

            private Vector2D GetDirection()
            {
                var x = Random.NextDouble() * 8000 - 4000;
                var y = Random.NextDouble() * 8000 - 4000;
                direction = new Vector2D(x, y);                
                return direction;
            }

            protected override void Animate(int frameNumber)
            {
                var currentTime = GetCurrentTime();
                double elapsedTime = currentTime - lastTime;

                var d = (elapsedTime / 1000) * direction;                

                pos = pos + d;
                
                // Change direction when we leave the bounds
                if(!InsideBounds(pos))                
                {
                    direction = GetDirection();
                }

                // Randomly change direction
                if(Random.Next(100) < 5 && Random.Next(100) < 3)
                {
                    direction = GetDirection();
                }

                lastTime = currentTime;
            }

            private bool InsideBounds(Point2D point)
            {
                return pos.X >= min.X && pos.Y >= min.Y && pos.X <= max.X && pos.Y <= max.Y;
            }            

            public override void MoveTo(DrawParams data)
            {
                base.MoveTo(data);                
                var customTransform = new Translation(pos.X, pos.Y);
                data.RenderContext.MultMatrixModelView(customTransform);
            }

            public override bool IsInFrustum(FrustumParams data, Point3D center, double radius)
            {
                // return true to avoid undesired clipping
                return true;
            }
        }
    }
}
