namespace HRYooba.Kinect.Core.Repositories
{
    public interface IAreaDataRepository
    {
        void Add(AreaData areaData);
        void Update(AreaData areaData);
        void Remove(int id);
        void Clear();
        AreaData Get(int id);
        AreaData[] GetAll();
    }
}