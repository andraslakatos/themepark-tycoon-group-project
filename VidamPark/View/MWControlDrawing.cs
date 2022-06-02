using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using Model;
using Persistence;
using MapModel.Building;
using Model.BuildingConstructors;
using System.Collections.Generic;
using System.Linq;
using MapModel.Facility;
using MapModel.Visitors;

namespace View
{
    public partial class MainWindow : GameWindow
    {
        #region Methods

        /// <summary>
        /// A játék Táblájának kirajzolása.
        /// </summary>
        private void DrawTable()
        {
            Shader shader = VisualData.GetShader(ShaderPosition.RelativeToCamera);
            shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            shader.SetMatrix4("view", _camera.GetViewMatrix());

            float[] vertices =
            {

                0f, 0f, 0, 0, 1,
                1f, 0f, 0, 1, 1,
                1f, 1f, 0, 1, 0,
                0f, 1f, 0, 0, 0,

            };
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices,
                BufferUsageHint.StaticDraw);




            float refMelysegX = (2f * _camera.Position.Z / _camera.GetProjectionMatrix().Column0[0]);
            float refMelysegY = (2f * _camera.Position.Z / _camera.GetProjectionMatrix().Column1[1]);

            int minX = (int) ((0 - 0.5f) * refMelysegX + _camera.Position.X);
            int minY = (int) ((0 - 0.5f) * refMelysegY - _camera.Position.Y);

            int maxX = (int) ((1 - 0.5f) * refMelysegX + _camera.Position.X);
            int maxY = (int) ((1 - 0.5f) * refMelysegY - _camera.Position.Y);

            if (minX < 0)
                minX = 0;
            if (minY < 0)
                minY = 0;

            maxX++;
            maxY++;
            if (maxX > _model.GameTable.TableSize)
                maxX = _model.GameTable.TableSize;
            if (maxY > _model.GameTable.TableSize)
                maxY = _model.GameTable.TableSize;
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices,
                BufferUsageHint.StaticDraw);

            


            bool poweredT = false;
            bool occupied = false;
            bool roadAround = false;
            if (mouseState == MouseClickState.Build)
            {
                bool posValid = true;
                int size = _model.GameTable.TableSize;
                if (_mousePositionToTable.X < 0 || _mousePositionToTable.Y < 0)
                {
                    posValid = false;
                    occupied = true;
                }

                if (_mousePositionToTable.X >= size || _mousePositionToTable.Y >= size)
                {
                    posValid = false;
                    occupied = true;
                }
                if (posValid)
                {
                    BuildingBaseData buildingTypeData = _model.BuildingData.GetBuildingType(BuildId);

                    bool water = (buildingTypeData is FacilityData && ((FacilityData)buildingTypeData).Water) ||
                                 buildingTypeData is PierData;

                    float XRadius = buildingTypeData.Size.X / 2.0f;
                    float YRadius = buildingTypeData.Size.Y / 2.0f;

                    float fMinX = (_mousePositionToTable.X - XRadius + 0.5f);
                    float fMinY = (_mousePositionToTable.Y - YRadius + 0.5f);

                    if (fMinX < 0)
                        fMinX--;
                    if (fMinY < 0)
                        fMinY--;

                    int mX = (int)fMinX;
                    int mY = (int)fMinY;
                    int MX = (int)(_mousePositionToTable.X + XRadius - 0.5f);
                    int MY = (int)(_mousePositionToTable.Y + YRadius - 0.5f);

                    for (int x = mX; x <= MX; x++)
                    {
                        if (x < 0 || x >= _model.GameTable.TableSize)
                        {
                            occupied = true;
                            break;
                        }

                        if (mY - 1 >= 0)
                        {
                            var tb1 = _model.GameTable[x, mY - 1].Building;
                            if (tb1 != null && (tb1 is Pier || tb1 is Road))
                            {
                                roadAround = true;
                            }
                        }

                        if (MY + 1 < _model.GameTable.TableSize)
                        {
                            var tb1 = _model.GameTable[x, MY + 1].Building;
                            if (tb1 != null && (tb1 is Pier || tb1 is Road))
                            {
                                roadAround = true;
                            }
                        }

                        if (roadAround)
                            break;
                    }

                    for (int y = mY; y <= MY; y++)
                    {
                        if (y < 0 || y >= _model.GameTable.TableSize)
                        {
                            occupied = true;
                            break;
                        }

                        if (mX - 1 >= 0)
                        {
                            var tb1 = _model.GameTable[mX - 1, y].Building;
                            if (tb1 != null && (tb1 is Pier || tb1 is Road))
                            {
                                roadAround = true;
                            }
                        }

                        if (MX + 1 < _model.GameTable.TableSize)
                        {
                            var tb1 = _model.GameTable[MX + 1, y].Building;
                            if (tb1 != null && (tb1 is Pier || tb1 is Road))
                            {
                                roadAround = true;
                            }
                        }

                        if (roadAround)
                            break;
                    }


                    for (int x = mX; x <= MX; x++)
                    {
                        if (x < 0 || x >= _model.GameTable.TableSize)
                        {
                            occupied = true;
                            break;
                        }

                        for (int y = mY; y <= MY; y++)
                        {
                            if (y < 0 || y >= _model.GameTable.TableSize)
                            {
                                occupied = true;
                                break;
                            }

                            if (((_model.GameTable[x, y].Base == TileType.Water) != water) ||
                                _model.GameTable[x, y].Building != null)
                            {
                                occupied = true;
                                break;
                            }
                            else if (_model.GameTable[x, y].Powered > 0)
                            {
                                poweredT = true;
                            }

                        }

                        if (occupied)
                            break;
                    }
                }
            }

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    switch (_model.GameTable[x, y].Base)
                    {
                        case TileType.Grass:
                            VisualData.UseTexture($"Grass{_model.GameTable[x, y].GrassType}");
                            break;
                        case TileType.Water:
                            VisualData.UseTexture(WaterType(_model.GameTable[x, y].Coords));
                            break;
                        default:
                            break;
                    }

                    shader.SetInt("isColor", 0);
                    Matrix4 position = Matrix4.CreateTranslation(x, -(y + 1), 0f);
                    shader.SetMatrix4("model", position);
                    GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);



                    if (mouseState == MouseClickState.Build)
                    {
                        float X = x + 0.5f;
                        float Y = y + 0.5f;
                        BuildingBaseData buildingTypeData = _model.BuildingData.GetBuildingType(BuildId);
                        float XRadius = buildingTypeData.Size.X / 2.0f;
                        float YRadius = buildingTypeData.Size.Y / 2.0f;
                        if (_mousePositionToTable.X - XRadius < X && _mousePositionToTable.X + XRadius > X)
                        {
                            if (_mousePositionToTable.Y - YRadius < Y && _mousePositionToTable.Y + YRadius > Y)
                            {
                                shader.SetInt("isColor", 1);
                                Color4 color = new Color4(30, 30, 170, 120);

                                if (buildingTypeData is FacilityData)
                                {
                                    if (!poweredT || !roadAround)
                                        color = new Color4(180, 180, 50, 120);
                                }
                                if (_model.GameTable.Balance < buildingTypeData.Price || occupied)
                                    color = new Color4(160, 25, 25, 120);

                                //color = new Color4(120, 72, 0, 80);
                                shader.SetColor4("color", color);
                                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt,
                                    0);
                            }
                        }
                    }
                    bool way = false;
                    if (_model.GameTable[x, y].Building is Road)
                    {
                        Road road = (Road)_model.GameTable[x, y].Building;
                        VisualData.UseTexture(RoadType(road.Coords));
                        position = Matrix4.CreateTranslation(road.Coords.X, -(road.Coords.Y + 1), 0f);
                        way = true;
                    }
                    else if (_model.GameTable[x, y].Building is Pier)
                    {
                        Pier pier = (Pier)_model.GameTable[x, y].Building;
                        BuildingBaseData buildingTypeData = _model.BuildingData.GetBuildingType(pier.BuildingTypeId);
                        VisualData.UseTexture(buildingTypeData.Name);
                        position = Matrix4.CreateTranslation(pier.Coords.X, -(pier.Coords.Y + 1), 0f);
                        way = true;
                    }
                    if (way)
                    {
                        shader.SetInt("isColor", 0);
                        shader.SetMatrix4("model", position);
                        float[] buildingVertices =
                        {
                            0, 0, 0, 0, 1, //bal alsó
                            1, 0, 0, 1, 1, //jobb alsó
                            1, 1, 0, 1, 0, //jobb felső
                            0, 1, 0, 0, 0, //bal felső
                        };

                        GL.BufferData(BufferTarget.ArrayBuffer, buildingVertices.Length * sizeof(float), buildingVertices,
                            BufferUsageHint.StaticDraw);
                        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
                    }

                }
            }

            DrawMice();

            DrawBuildings();

        }

        /// <summary>
        /// A táblán belüli épületek kirajzolása.
        /// </summary>
        private void DrawBuildings()
        {
            Shader shader = VisualData.GetShader(ShaderPosition.RelativeToCamera);
            shader.SetInt("isColor", 0);
            for (int i = 0; i < _model.GameTable.Buildings.Count; i++)
            {

                Buildings b = _model.GameTable.Buildings[i];
                if (b is Road || b is Pier)
                    continue;
                BuildingBaseData buildingTypeData = _model.BuildingData.GetBuildingType(b.BuildingTypeId);
                var cSize = buildingTypeData.Size;

                VisualData.UseTexture(buildingTypeData.Name);



                Matrix4 position = Matrix4.CreateTranslation(b.Coords.X, -(b.Coords.Y + 1), 0f);
                shader.SetMatrix4("model", position);
                float[] buildingVertices =
                {
                    0f, 1 - cSize.Y, 0, 0, 1, //bal alsó
                    cSize.X, 1 - cSize.Y, 0, 1, 1, //jobb alsó
                    cSize.X, 1f, 0, 1, 0, //jobb felső
                    0f, 1f, 0, 0, 0, //bal felső
                };

                GL.BufferData(BufferTarget.ArrayBuffer, buildingVertices.Length * sizeof(float), buildingVertices,
                    BufferUsageHint.StaticDraw);
                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            }

            VisualData.UseTexture("UnderConstruction");
            for (int i = 0; i < _model.GameTable.UnderConstructionList.Count; i++)
            {

                Buildings b = _model.GameTable.UnderConstructionList[i];

                BuildingBaseData buildingTypeData = _model.BuildingData.GetBuildingType(b.BuildingTypeId);
                var cSize = buildingTypeData.Size;

                Matrix4 position = Matrix4.CreateTranslation(b.Coords.X, -(b.Coords.Y + 1), 0f);
                shader.SetMatrix4("model", position);
                float[] buildingVertices =
                {
                    0f, 1 - cSize.Y, 0, 0, 1, //bal alsó
                    cSize.X, 1 - cSize.Y, 0, 1, 1, //jobb alsó
                    cSize.X, 1f, 0, 1, 0, //jobb felső
                    0f, 1f, 0, 0, 0, //bal felső
                };

                GL.BufferData(BufferTarget.ArrayBuffer, buildingVertices.Length * sizeof(float), buildingVertices,
                    BufferUsageHint.StaticDraw);
                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }

        /// <summary>
        /// Az "egér" látogatók kirajzolása a táblán.
        /// </summary>
        private void DrawMice()
        {
            Shader shader = VisualData.GetShader(ShaderPosition.RelativeToCamera);
            VisualData.UseTexture("Mouse");
            shader.SetInt("isColor", 2);
            float ms = 0.15f;
            float[] MouseVertices =
            {

                -ms, -ms, 0, 0, 1,
                ms, -ms, 0, 1, 1,
                ms, ms, 0, 1, 0,
                -ms, ms, 0, 0, 0,

            };
            GL.BufferData(BufferTarget.ArrayBuffer, MouseVertices.Length * sizeof(float), MouseVertices,
                BufferUsageHint.StaticDraw);
            if (_model.GameTable.Visitors != null)
            {
                for (int i = _model.GameTable.Visitors.Count-1; i >=0 ; i--)
                {
                    Visitor v = _model.GameTable.Visitors[i];

                    if(v.CurrentActivity == Activity.Waiting || v.CurrentActivity == Activity.Using)
                    {
                        continue;
                    }


                    shader.SetVector4("color", new Vector4(v.SkinColor.X, v.SkinColor.Y, v.SkinColor.Z, v.SkinColor.W));
                    Matrix4 position;

                    Vector2? qmPos = GetMousePrecisionPoint(v);
                    if (qmPos != null)
                    {
                        Vector2 mPos = qmPos.Value;
                        position = Matrix4.CreateTranslation(mPos.X,mPos.Y,0);
                        shader.SetMatrix4("model", position);
                        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Látogató pontos pozíciójának meghatározása
        /// </summary>
        /// <param name="v">Látogató aminely a pontos helyét tudni akarjuk</param>
        /// <returns>Pontos pozíciója a látogatónak</returns>
        private Vector2? GetMousePrecisionPoint(Visitor v)
        {
            Vector2 position;

            Vector2 diff = new Vector2(v.NextTile.X - v.Coords.X, v.NextTile.Y - v.Coords.Y);

            Vector2 inTileDiff = new Vector2(v.InTileCoords.X, v.InTileCoords.Y);
            if (v.CurrentBuilding != null && v.CurrentBuilding == _model.GameTable[v.NextTile.X, v.NextTile.Y].Building && v.CurrentActivity == Activity.Moving)
            {
                diff -= inTileDiff;
                diff.X *= v.MoveTick;
                diff.X /= Visitor.MOVEREQUIRED;

                diff.Y *= v.MoveTick;
                diff.Y /= Visitor.MOVEREQUIRED;
                diff += inTileDiff;
                position = new Vector2(v.Coords.X + diff.X + 0.5f, -(v.Coords.Y + 1 + diff.Y - 0.5f));
            }
            else if (_model.GameTable[v.Coords.X, v.Coords.Y].Building is Facilities)
            {
                if (v.Coords == v.NextTile)
                    return null;
                diff += inTileDiff;
                diff.X *= v.MoveTick;
                diff.X /= Visitor.MOVEREQUIRED;

                diff.Y *= v.MoveTick;
                diff.Y /= Visitor.MOVEREQUIRED;
                position = new Vector2(v.Coords.X + diff.X + 0.5f, -(v.Coords.Y + 1 + diff.Y - 0.5f));
            }
            else
            {
                diff.X *= v.MoveTick;
                diff.X /= Visitor.MOVEREQUIRED;

                diff.Y *= v.MoveTick;
                diff.Y /= Visitor.MOVEREQUIRED;
                position = new Vector2(v.Coords.X + diff.X + 0.5f + v.InTileCoords.X, -(v.Coords.Y + 1 + diff.Y - 0.5f + v.InTileCoords.Y));
            }
            return position;
        }
        
        /// <summary>
        /// A környezetétől függően megadja, milyen irányú út való oda.
        /// </summary>
        /// <param name="p">táblán belüli koordináta</param>
        /// <returns>neve az útnak amivel megfeletethető a típusa</returns>
        private string RoadType(Point p)
        {
            TileType xy1;
            TileType x1y;
            
            string name = "Road";
            TileType tS = TileType.GeneratorSmall;
            TileType tM = TileType.GeneratorMedium;
            TileType tL = TileType.GeneratorLarge;
            if (IsValid(p.X, p.Y - 1))
            {
                xy1 = _model.GameTable[p.X, p.Y - 1].BuildingType;
                if (xy1 != TileType.Empty && xy1!=tS && xy1 != tM && xy1 != tL) { name += "_up"; }
            }
            if (IsValid(p.X + 1, p.Y))
            { 
                x1y = _model.GameTable[p.X + 1, p.Y].BuildingType;
                if (x1y != TileType.Empty && x1y != tS && x1y != tM && x1y != tL) { name += "_right"; }
            }
            if (IsValid(p.X, p.Y + 1))
            {
                xy1 = _model.GameTable[p.X, p.Y + 1].BuildingType;
                if (xy1 != TileType.Empty && xy1 != tS && xy1 != tM && xy1 != tL) { name += "_down"; }
            }
            if (IsValid(p.X - 1, p.Y))
            {
                x1y = _model.GameTable[p.X - 1, p.Y].BuildingType;
                if (x1y != TileType.Empty && x1y!= tS && x1y != tM && x1y != tL) { name += "_left"; }
            }
            if (name == "Road") { name += "_"; }
            return name;
        }

        /// <summary>
        /// A környezetétől függően megadja, milyen irányú víz való oda.
        /// </summary>
        /// <param name="p">táblán belüli koordináta</param>
        /// <returns>neve a víznek amivel megfeletethető a típusa</returns>
        private string WaterType(Point p)
        {
            TileType xy1;
            TileType x1y;

            string name = "Water";

            if (IsValid(p.X, p.Y - 1))
            {
                xy1 = _model.GameTable[p.X, p.Y - 1].Base;
                if (xy1 == TileType.Water) { name += "_up"; }
            }
            if (IsValid(p.X + 1, p.Y))
            {
                x1y = _model.GameTable[p.X + 1, p.Y].Base;
                if (x1y == TileType.Water) { name += "_right"; }
            }
            if (IsValid(p.X, p.Y + 1))
            {
                xy1 = _model.GameTable[p.X, p.Y + 1].Base;
                if (xy1 == TileType.Water) { name += "_down"; }
            }
            if (IsValid(p.X - 1, p.Y))
            {
                x1y = _model.GameTable[p.X - 1, p.Y].Base;
                if (x1y == TileType.Water) { name += "_left"; }
            }
            if (name == "Water") { name += "_"; }
            return name;
        }

        /// <summary>
        /// A táblára nézve valid-e a megadott koordináta.
        /// </summary>
        /// <param name="x">x koordináta</param>
        /// <param name="y">y koordináta</param>
        /// <returns>igaz ha valid a pont</returns>
        private bool IsValid(int x, int y)
        {
            int tSize = _model.GameTable.TableSize;
            if (x < 0 || y < 0 || x >= tSize || y >= tSize) return false;
            return true;
        }

        /// <summary>
        /// Vezérlőről eldönti, hogy milyen típus és tovább adja a kirajzolásnak.
        /// </summary>
        /// <param name="control">vezérlő amit ki akarunk rajzolni</param>
        private void DrawControl(Control control)
        {
            if (control.IsVisible)
            {


                switch (control.GetControlType())
                {
                    case ControlType.Image:
                        DrawImage(control);
                        break;
                    case ControlType.Button:
                        DrawButton((Button)control);
                        break;
                    case ControlType.Textbox:
                        DrawTextBox((TextBox)control);
                        break;
                    case ControlType.BuildMenu:
                        DrawBuildMenu((BuildMenuControl)control);
                        break;
                    case ControlType.Label:
                        DrawLabel((Label)control);
                        break;
                }

            }
        }
        /// <summary>
        /// Cím kirajzolás.
        /// </summary>
        /// <param name="control">ezt a címet rajzoljuk ki</param>
        private void DrawLabel(Label control)
        {


            var shader = control.GetShader();
            shader.Use();
            control.DisplayId = _displayedImages++;


            shader.SetInt("isColor", 1);
            Color4? color = ((ImageControl)control).BackgroundColor;
            if (color != null)
                shader.SetColor4("color", color.Value);
            else
                shader.SetColor4("color", new Color4(0, 0, 0, 0));



            GL.BindVertexArray(_vertexArrayObject);
            GL.BufferData(BufferTarget.ArrayBuffer, control.Vertices.Length * sizeof(float), control.Vertices, BufferUsageHint.StaticDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            control.GetTexture().Use();
            shader.SetInt("isColor", 0);
            GL.BindVertexArray(_vertexArrayObject);
            GL.BufferData(BufferTarget.ArrayBuffer, control.Vertices.Length * sizeof(float), control.Vertices, BufferUsageHint.StaticDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        /// <summary>
        /// építésmenü kirajzolás.
        /// </summary>
        /// <param name="control">ezt a építésmenü rajzoljuk ki</param>
        private void DrawBuildMenu(BuildMenuControl bmc)
        {
            var shader = bmc.GetShader();
            shader.Use();

            if (bmc.DrawType == DrawType.Color)
            {
                shader.SetInt("isColor", 1);
                Color4? color = ((ImageControl)bmc).BackgroundColor;
                if (color != null)
                    shader.SetColor4("color", color.Value);
                else
                    shader.SetColor4("color", new Color4(0, 0, 0, 0));
            }
            else if (bmc.DrawType == DrawType.Image)
            {
                shader.SetInt("isColor", 0);
                var tex = bmc.GetTexture();
                tex.Use();
            }
            bmc.DisplayId = _displayedImages++;

            bool hidden = false;

            List<Control> buildList = new List<Control>();
            List<Control> buildInfoList = new List<Control>();
            switch (BuildMenu.activeBuildList)
            {
                case ActiveBuildList.Roads:
                    buildList = BuildMenu.RoadsList;
                    buildInfoList = BuildMenu.RoadsInfoList;
                    break;
                case ActiveBuildList.Rides:
                    buildList = BuildMenu.RidesList;
                    buildInfoList = BuildMenu.RidesInfoList;
                    break;
                case ActiveBuildList.Utilities:
                    buildList = BuildMenu.UtilitiesList;
                    buildInfoList = BuildMenu.UtilitiesInfoList;
                    break;
                case ActiveBuildList.Generators:
                    buildList = BuildMenu.GeneratorsList;
                    buildInfoList = BuildMenu.GeneratorsInfoList;
                    break;
                case ActiveBuildList.Hide:
                    hidden = true;
                    break;
                default:
                    break;
            }
            bmc.Children.Clear();
            bmc.Children.Add(BuildMenu.Roads);
            bmc.Children.Add(BuildMenu.Rides);
            bmc.Children.Add(BuildMenu.Utilities);
            bmc.Children.Add(BuildMenu.Generators);
            if (hidden)
            {
                bmc.Position = new Point(0, BuildMenu.HideHeight);
                if (bmc.Children.Contains(BuildMenu.Hide))
                    bmc.Children.Remove(BuildMenu.Hide);
            }
            else
            {
                bmc.Position = new Point(0, 0);
                if (!bmc.Children.Contains(BuildMenu.Hide))
                    bmc.Children.Add(BuildMenu.Hide);
            }
            GL.BindVertexArray(_vertexArrayObject);
            GL.BufferData(BufferTarget.ArrayBuffer, bmc.Vertices.Length * sizeof(float), bmc.Vertices, BufferUsageHint.StaticDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            for (int i = 0; i < buildList.Count; i++)
            {
                var buildingButton = buildList[i];
                bmc.Children.Add(buildingButton);

                if (buildingButton.Vertices[5] > _mousePositionRaw.X && buildingButton.Vertices[0] < _mousePositionRaw.X)
                {

                    if (buildingButton.Vertices[6] < _mousePositionRaw.Y && buildingButton.Vertices[11] > _mousePositionRaw.Y)
                    {
                        bmc.Children.Add(buildInfoList[i]);
                    }
                }

                

            }
            foreach (var child in bmc.Children)
            {
                DrawControl(child);
            }


        }

        /// <summary>
        /// szövegdoboz kirajzolás.
        /// </summary>
        /// <param name="control">ezt a szövegdoboz rajzoljuk ki</param>
        private void DrawTextBox(TextBox textbox)
        {
            var shader = textbox.GetShader();
            shader.Use();

            if (textbox.DrawType == DrawType.Color)
            {
                shader.SetInt("isColor", 1);
                Color4? color = ((ImageControl)textbox).BackgroundColor;
                if (color != null)
                    shader.SetColor4("color", color.Value);
                else
                    shader.SetColor4("color", new Color4(0, 0, 0, 0));
            }
            else if (textbox.DrawType == DrawType.Image)
            {
                shader.SetInt("isColor", 0);
                var tex = textbox.GetTexture();
                tex.Use();
            }


            GL.BindVertexArray(_vertexArrayObject);
            GL.BufferData(BufferTarget.ArrayBuffer, textbox.Vertices.Length * sizeof(float), textbox.Vertices, BufferUsageHint.StaticDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            textbox.DisplayId = _displayedImages++;

            Label content = textbox.Content;
            shader.SetInt("isColor", 0);
            content.GetTexture().Use();



            GL.BindVertexArray(_vertexArrayObject);
            GL.BufferData(BufferTarget.ArrayBuffer, content.Vertices.Length * sizeof(float), content.Vertices, BufferUsageHint.StaticDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            if (!textbox.IsReadOnly && textbox.Caret.IsVisible)
            {
                shader.SetInt("isColor", 1);
                shader.SetColor4("color", textbox.Caret.GetCurrentColor());

                GL.BindVertexArray(_vertexArrayObject);
                GL.BufferData(BufferTarget.ArrayBuffer, textbox.Caret.Vertices.Length * sizeof(float), textbox.Caret.Vertices, BufferUsageHint.StaticDraw);
                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            }


        }

        /// <summary>
        /// gomb kirajzolás.
        /// </summary>
        /// <param name="control">ezt a gomb rajzoljuk ki</param>
        private void DrawButton(Button control)
        {
            var shader = control.GetShader();
            shader.Use();

            if (control.DrawType == DrawType.Color)
            {
                shader.SetInt("isColor", 1);
                Color4? color = ((ImageControl)control).BackgroundColor;
                if (color != null)
                    shader.SetColor4("color", color.Value);
                else
                    shader.SetColor4("color", new Color4(0, 0, 0, 0));
            }
            else if (control.DrawType == DrawType.Image)
            {
                shader.SetInt("isColor", 0);
                var tex = control.GetTexture();
                tex.Use();
            }


            GL.BindVertexArray(_vertexArrayObject);
            GL.BufferData(BufferTarget.ArrayBuffer, control.Vertices.Length * sizeof(float), control.Vertices, BufferUsageHint.StaticDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            control.DisplayId = _displayedImages++;

            Label tc = control.TextControl;
            if (tc.haveTexture)
            {
                tc.GetShader().Use();
                tc.GetTexture().Use();

                var c = ((SolidBrush)control.Brush).Color;

                GL.BindVertexArray(_vertexArrayObject);
                GL.BufferData(BufferTarget.ArrayBuffer, tc.Vertices.Length * sizeof(float), tc.Vertices, BufferUsageHint.StaticDraw);
                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            }


        }
        /// <summary>
        /// kép kirajzolás.
        /// </summary>
        /// <param name="control">ezt a kép rajzoljuk ki</param>
        private void DrawImage(Control control)
        {


            var shader = control.GetShader();
            shader.Use();

            if (control.DrawType == DrawType.Color)
            {
                shader.SetInt("isColor", 1);
                Color4? color = ((ImageControl)control).BackgroundColor;
                if (color != null)
                    shader.SetColor4("color", color.Value);
                else
                    shader.SetColor4("color", new Color4(0, 0, 0, 0));
            }
            else if (control.DrawType == DrawType.Image)
            {
                shader.SetInt("isColor", 0);
                var tex = control.GetTexture();
                tex.Use();
            }
            control.DisplayId = _displayedImages++;


            GL.BindVertexArray(_vertexArrayObject);
            GL.BufferData(BufferTarget.ArrayBuffer, control.Vertices.Length * sizeof(float), control.Vertices, BufferUsageHint.StaticDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            foreach (var child in control.Children)
            {
                DrawControl(child);
            }
        }
        #endregion
    }
}
