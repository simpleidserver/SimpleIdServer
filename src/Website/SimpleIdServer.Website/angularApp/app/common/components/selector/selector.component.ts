import { Component, Input } from '@angular/core';

@Component({
    selector: 'selector',
    templateUrl: './selector.component.html'
})
export class SelectorComponent {
    private _sourceValues : Array<String> = [];
    private _targetValues : Array<String> = [];
    filteredSourceValues : Array<String> = [];
    @Input() 
    set sourceValues(val : Array<String>) {
        this._sourceValues = val;
        this.filteredSourceValues = this._sourceValues.filter(_ => {
            const index = this._targetValues.indexOf(_);
            return index === -1;
        });
    }

    @Input() 
    set targetValues(val : Array<String>) {
        this._targetValues = val;
        this.filteredSourceValues = this._sourceValues.filter(_ => {
            const index = this._targetValues.indexOf(_);
            return index === -1;
        });
    }
    
    get targetValues() : Array<String> {
        return this._targetValues;
    }

    selectedSourceValues : Array<String> = [];
    selectedTargetValues : Array<String> = [];

    add() {
        this.internalRemove(this.selectedSourceValues, this.filteredSourceValues, this._targetValues);
    }

    remove() {
        this.internalRemove(this.selectedTargetValues, this._targetValues, this.filteredSourceValues);
    }

    private internalRemove(selected : Array<String>, source : Array<String>, target : Array<String>) : void {
        const self = this;
        const indexes = [];
        selected.forEach(elt => {
            const index = source.indexOf(elt);
            indexes.push(index);
            target.push(elt);
        });

        indexes.sort((a, b) => b - a);
        for(var i = 0; i < indexes.length; i++) {
            source.splice(indexes[i], 1);
        }
    }
}