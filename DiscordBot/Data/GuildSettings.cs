﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Discord.Rest;

namespace Botflox.Bot.Data
{
    public class GuildSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong GuildId { get; set; }

        [StringLength(5, MinimumLength = 1)]
        public string CommandPrefix { get; set; }
    }
}