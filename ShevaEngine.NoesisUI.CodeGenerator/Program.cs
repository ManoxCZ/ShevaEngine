using ShevaEngine.NoesisUI.CodeGenerator;
using System;

class Program
{
    static void Main(string[] args)
    {
        VertexSourceGenerator vertexSourceGenerator = new();
        vertexSourceGenerator.Execute("../../../../../ShevaEngine.NoesisUI/Generated");
    }
}