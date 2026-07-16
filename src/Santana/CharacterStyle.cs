namespace Santana
{
    public struct CharacterStyle
    {
        public uint Value => this;

        public CharacterGender Gender { get; set; }
        public byte Slot { get; set; }

        public CharacterStyle(uint value)
        {
            Gender = (CharacterGender)(value & 1);
            Slot = (byte)(value >> 28);
        }

        public CharacterStyle(CharacterGender gender, byte slot)
        {
            Gender = gender;
            Slot = (byte)(slot & 0x0F);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator uint(CharacterStyle style)
        {
            var value = (uint)style.Gender | ((uint)(style.Slot & 0x0F) << 28);
            return value;
        }

        public static implicit operator CharacterStyle(uint value)
        {
            return new CharacterStyle(value);
        }
    }
}
