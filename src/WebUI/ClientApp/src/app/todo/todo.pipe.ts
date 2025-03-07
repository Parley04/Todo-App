import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'todopipe'
})
export class TodoPipe implements PipeTransform {
  
  transform(todoItems: any[], searchText: string): any[] {
    if (!todoItems || !searchText) {
      return todoItems;
    }

    searchText = searchText.toLowerCase();

    return todoItems.filter(item => 
      item.title?.toLowerCase().includes(searchText) || 
      item.note?.toLowerCase().includes(searchText) ||
      item.itemTags?.some(tag => tag.tags?.some(t => t.name?.toLowerCase().includes(searchText)))
    );
  }
}
