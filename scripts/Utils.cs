using Godot;

public class Utils
{
    public static float AngleBetween(Vector3 u, Vector3 v)
    {
        return Mathf.Acos(u.Dot(v) / (u.Length() * v.Length()));
    }

    public static float OrientedAngleBetween(Vector3 u, Vector3 v, Vector3 n)
    {
        float angle = AngleBetween(u, v);
        Vector3 crossProduct = u.Cross(v);
        if (n.Dot(crossProduct) > 0)
        {
            angle = -angle;
        }
        return angle;
    }

    public static float Rad2Deg(float rad)
    {
        return rad * (180 / Mathf.Pi);
    }

    public static bool MoreOrLessEqual(float a, float b, float epsilon)
    {
        return a >= b - epsilon && a <= b + epsilon;
    }

    public static MeshInstance3D DrawLine(Vector3 start, Vector3 end, Color color)
    {
        MeshInstance3D meshInstance = new();
        ImmediateMesh immediateMesh = new();
        OrmMaterial3D material = new();

        meshInstance.Mesh = immediateMesh;
        meshInstance.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

        immediateMesh.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
        immediateMesh.SurfaceAddVertex(start);
        immediateMesh.SurfaceAddVertex(end);
        immediateMesh.SurfaceEnd();

        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        material.AlbedoColor = color;

        return meshInstance;
    }
}