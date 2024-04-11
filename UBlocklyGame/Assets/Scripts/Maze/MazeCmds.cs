using System.Collections;
using UBlockly;

namespace UBlocklyGame.Maze
{
    [CodeInterpreter(BlockType = "maze_move")]
    public class Maze_Move_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            CmdEnumerator ctor = CSharp.Interpreter.ValueReturn(block, "NUM", new DataStruct(0));
            yield return ctor;
            DataStruct arg0 = ctor.Data;

            float dirValue = arg0.NumberValue.Value;
            yield return MazeController.Instance.DoMoveForward(dirValue);
        }
    }

    [CodeInterpreter(BlockType = "maze_move_by_line")]
    public class Maze_Move_By_Line_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            yield return MazeController.Instance.DoMoveByLine();
        }
    }

    [CodeInterpreter(BlockType = "maze_turn")]
    public class Maze_Turn_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            string dirStr = block.GetFieldValue("DIRECTION");
            CmdEnumerator ctor = CSharp.Interpreter.ValueReturn(block, "NUM", new DataStruct(0));
            yield return ctor;
            DataStruct arg0 = ctor.Data;
            float dirValue = arg0.NumberValue.Value;
            Direction dir = dirStr.Equals("LEFT") ? Direction.Left : Direction.Right;
            yield return MazeController.Instance.DoTurn(dir, dirValue);
        }
    }

    [CodeInterpreter(BlockType = "maze_bool_access")]
    public class Maze_BoolAccess_Cmdtor : ValueCmdtor
    {
        protected override DataStruct Execute(Block block)
        {
            string dirStr = block.GetFieldValue("ACCESS");
            Direction dir = Direction.Front;
            switch (dirStr)
            {
                case "FRONT":
                    dir = Direction.Front;
                    break;
                case "RIGHT":
                    dir = Direction.Right;
                    break;
                case "LEFT":
                    dir = Direction.Left;
                    break;
            }
            bool access = MazeController.Instance.DoCheckAccess(dir);
            return new DataStruct(access);
        }
    }
    
    [CodeInterpreter(BlockType = "maze_reach_terminal")]
    public class Maze_ReachTerminal_Cmdtor : ValueCmdtor
    {
        protected override DataStruct Execute(Block block)
        {
            bool reach = MazeController.Instance.DoCheckAccomplish();
            return new DataStruct(reach);
        }
    }

    [CodeInterpreter(BlockType = "maze_line_below")]
    public class Maze_LineBelow_Cmdtor : ValueCmdtor
    {
        protected override DataStruct Execute(Block block)
        {
            bool reach = MazeController.Instance.mAvatar.IsLineBelow();
            return new DataStruct(reach);
        }
    }
}