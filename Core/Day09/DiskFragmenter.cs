using System.Diagnostics;
using System.Text;
using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day09;

public class DiskFragmenter : BaseDayModule
{
    public DiskFragmenter(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    public override int Day => 9;

    public override string Title => "Disk Fragmenter";

    [Fact, ShowDebug] public void Part1_Sample() => ExecutePart1(GetData(InputType.Sample)).Should().Be(1928);
    [Fact] public void Part1() => ExecutePart1(GetData(InputType.Input));

    [Fact, ShowDebug] public void Part2_Sample() => ExecutePart2(GetData(InputType.Sample)).Should().Be(2858);
    [Fact] public void Part2() => ExecutePart2(GetData(InputType.Input));

    private class DiskBlock
    {
        public long? FileId { get; set; }

        public DiskBlock(long? FileId)
        {
            this.FileId = FileId;
        }
        
        public bool IsEmpty => !FileId.HasValue;
    }

    private string Visualize(List<DiskBlock> blocks)
    {
        var output = blocks.Select(x => x.FileId == null ? "." : $"[{x.FileId}]").JoinWith("");
        return output;
    }
    
    private List<DiskBlock> LoadDiskMap(string data)
    {
        data = data.ToLines(removeEmptyLines: true).First(); // make sure we don't get the carriage return at the end of the puzzle input
        var fileMode = true;
        var fileId = 0L;
        var output = new List<DiskBlock>();
        for (int i = 0; i < data.Length; i++)
        {
            var length = int.Parse(data[i].ToString());
            Enumerable.Range(0, length).ToList().ForEach(_ => output.Add(new DiskBlock(fileMode ? fileId : null)));
            if (fileMode)
            {
                fileId++;
            }
            fileMode = !fileMode;
        }
        return output;
    }
    
    public long ExecutePart1(string data)
    {
        var diskMap = LoadDiskMap(data);
        
        Debug($"Loaded Disk:  {Visualize(diskMap)}");
        
        DefragmentBySingleBlocks(diskMap);
        
        Debug($"Defragmented: {Visualize(diskMap)}");

        var solution = Checksum(diskMap);
        WriteLine($"Defragmented Checksum: {solution}");
        return solution;
    }

    private void DefragmentBySingleBlocks(List<DiskBlock> disk)
    {
        // move from right to left
        for (int i = (disk.Count-1); i >= 0; i--)
        {
            var value = disk[i].FileId;
            if (value != null)
            {
                // find the first empty block
                var firstEmptyIndex = disk.FindIndex(x => x.IsEmpty);
                if (firstEmptyIndex < i)
                {
                    // move the value to the first empty block
                    disk[firstEmptyIndex].FileId = value;
                    disk[i].FileId = null;
                }
            }
        }
    }

    private long Checksum(List<DiskBlock> disk)
    {
        var checksum = disk
            .Select((block, pos) => (block.FileId ?? 0) * pos)
            .Sum();
        return checksum;
    }
    
    public long ExecutePart2(string data)
    {
        var diskMap = LoadDiskMap(data);
        
        Debug($"Loaded Disk: {Visualize(diskMap)}");
        
        DefragmentByFile(diskMap);
        
        Debug($"File Defrag: {Visualize(diskMap)}");

        var solution = Checksum(diskMap);
        WriteLine($"Defragmented Checksum: {solution}");
        return solution;
    }
    
    private void DefragmentByFile(List<DiskBlock> disk)
    {
        var maxFileId = disk.Max(x => x.FileId) ?? 0;
        for (long fileId = maxFileId; fileId >= 0; fileId--)
        {
            var indexesOfCurrentFile = disk
                .Select((block, pos) => (block, pos))
                .Where(x => x.block.FileId == fileId)
                .Select(x => x.pos).ToList();

            var lengthOfFile = indexesOfCurrentFile.Count;
            var startIndexOfFile = indexesOfCurrentFile.First();
            
            // find the first empty block large enough to hold the file, left of the file
            for (int i = 0; i <= startIndexOfFile; i++)
            {
                var indexesToCheck = Enumerable.Range(i, lengthOfFile).ToList();
                if (indexesToCheck.All(x => disk[x].IsEmpty))
                {
                    // move the file to the empty blocks
                    for (int j = 0; j < lengthOfFile; j++)
                    {
                        disk[indexesToCheck[j]].FileId = fileId;
                        disk[startIndexOfFile + j].FileId = null;
                    }
                    break;
                }
            }
        }
    }

}

