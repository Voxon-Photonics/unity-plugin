using UnityEngine;

namespace Voxon
{
    public class VXVoxel_ : IDrawable
    {
        VoxelView _voxelView;

        public VXVoxel_(Vector3 pos, Color32 col) : this(pos, col, null)
        {
        }

        public VXVoxel_(Vector3 pos, Color32 col, GameObject parent)
        {
            _voxelView = new VoxelView(pos, col, parent);
            VXProcess.Drawables.Add(this);
        }

        public void Update()
        {
        }

        public void Draw()
        {
            _voxelView.Draw();
        }
    }
}
