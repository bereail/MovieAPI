﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MOVIE_API.Models.Enum;

namespace MOVIE_API.Models;

public partial class Movie
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    [MaxLength(255)]
    [Column("director")]
    public string Director { get; set; }

    [ForeignKey("IdAdmin")]
    public int IdAdmin { get; set; }

    [Column(TypeName = "state")]
    public MovieState? State { get; set; }

    [MaxLength(255)]
    [Column("title")]
    public string Title { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Admin IdAdminNavigation { get; set; }

}





