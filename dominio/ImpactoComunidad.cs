namespace dominio
{
    // Totales de toda la comunidad (los muestra la portada)
    public class ImpactoComunidad
    {
        public int ObjetosEntregados { get; set; }
        public decimal KgEvitados { get; set; }     // estimado: peso por categoría x objetos entregados
        public int Estudiantes { get; set; }
    }
}
