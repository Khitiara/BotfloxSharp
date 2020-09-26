using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Botflox.Bot.Data
{
    public class UserSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong UserId { get; set; }

        public ulong MainCharacter { get; set; }

        public bool VerifiedCharacter { get; set; }
    }
}