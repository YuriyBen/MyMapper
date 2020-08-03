using MyMapper.Models;
using MyMapper.Entities;
namespace MyMapper.Extensions
{
	public static class NoteMapperExtensions
	{
		public static Note ToNote(this NoteDTO noteDTO)
		{
			Note note = new Note(); 
			note.Id = noteDTO.Id;
			note.DateId = noteDTO.DateId;
			note.Comment = noteDTO.Comment;
			return note;
		}
		public static NoteDTO ToNoteDTO(this Note note)
		{
			NoteDTO noteDTO = new NoteDTO(); 
			noteDTO.Id = note.Id;
			noteDTO.DateId = note.DateId;
			noteDTO.Comment = note.Comment;
			return noteDTO;
		}
	}
}
