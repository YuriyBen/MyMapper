using System;
using MyMapper.Models;
using MyMapper.Entities;
namespace MyMapper.Extensions
{
   public static class NoteDTOExtensions
   {
      public static Note ToNote(this NoteDTO noteDTO)
      {
		Note note = new Note(); 
		note.Id = noteDTO.Id;
		note.DateId = noteDTO.DateId;
		note.Comment = noteDTO.Comment;
		return note;
      }
   }
}
