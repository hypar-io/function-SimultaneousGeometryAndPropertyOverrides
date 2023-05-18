using Elements;
using Elements.Geometry;
using System.Collections.Generic;

namespace SimultaneousGeometryAndPropertyOverrides
{
    public static class SimultaneousGeometryAndPropertyOverrides
    {
        /// <summary>
        /// The SimultaneousGeometryAndPropertyOverrides function.
        /// </summary>
        /// <param name="model">The input model.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A SimultaneousGeometryAndPropertyOverridesOutputs instance containing computed results and the model with any new elements.</returns>
        public static SimultaneousGeometryAndPropertyOverridesOutputs Execute(Dictionary<string, Model> inputModels, SimultaneousGeometryAndPropertyOverridesInputs input)
        {
            var output = new SimultaneousGeometryAndPropertyOverridesOutputs();

            var walls = Polygon.Star(10, 7, 5).Segments().Select(s => new StandardWall(s, 0.1, 1) { AdditionalProperties = new Dictionary<string, object>() { { "Add Id", $"{s.Start}, {s.End}" } } }).ToList();

            walls = input.Overrides.Walls.CreateElements(
              input.Overrides.Additions.Walls,
              input.Overrides.Removals.Walls,
              (add) => CreateWall(add),
              (wall, ident) => wall.AdditionalProperties["Add Id"].ToString() == ident.AddId,
              (wall, edit) => UpdateWall(wall, edit),
              walls
            );


            output.Model.AddElements(walls);

            return output;
        }

        private static StandardWall UpdateWall(StandardWall wall, WallsOverride edit)
        {
            var newWall = new StandardWall(edit.Value.CenterLine ?? wall.CenterLine, Math.Max(edit.Value.Thickness, 0.1), Math.Max(edit.Value.Height, 0.1)) { AdditionalProperties = new Dictionary<string, object>() { { "Add Id", edit.Id } } };
            return newWall;
        }

        private static StandardWall CreateWall(WallsOverrideAddition add)
        {
            return new StandardWall(add.Value.CenterLine, Math.Max(add.Value.Thickness, 0.1), Math.Max(add.Value.Height, 0.1)) { AdditionalProperties = new Dictionary<string, object>() { { "Add Id", add.Id } } };
        }
    }
}