using Godot;


namespace GameGeneral
{
    public class CompositeEffect : Spatial
    {
        public void Play()
        {
            foreach (Node effect in GetChildren())
            {
                if (effect is Particles)
                {
                    ((Particles)effect).Emitting = true;
                }
                else if (effect is AudioStreamPlayer3D)
                {
                    ((AudioStreamPlayer3D)effect).Play();
                }
            }
        }
    }
}

