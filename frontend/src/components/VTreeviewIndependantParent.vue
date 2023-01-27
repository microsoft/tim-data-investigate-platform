<script>
import { VTreeview } from 'vuetify/lib/components';
import { getObjectValueByPath } from 'vuetify/lib/util/helpers';

export default {
  mixins: [VTreeview],
  methods: {
    updateSelected(key, isSelected, isForced = false) {
      // eslint-disable-next-line no-prototype-builtins
      if (!this.nodes.hasOwnProperty(key)) return;

      const changed = new Map();

      if (this.selectionType !== 'independent') {
        // eslint-disable-next-line no-restricted-syntax
        for (const descendant of this.getDescendants(key)) {
          if (!getObjectValueByPath(this.nodes[descendant].item, this.itemDisabled) || isForced) {
            this.nodes[descendant].isSelected = isSelected;
            this.nodes[descendant].isIndeterminate = false;
            changed.set(descendant, isSelected);
          }
        }

        const calculated = this.calculateState(key, this.nodes);
        this.nodes[key].isSelected = isSelected;
        this.nodes[key].isIndeterminate = calculated.isIndeterminate;
        changed.set(key, isSelected);

        // Prevent orphan children, by unselecting the parents
        // eslint-disable-next-line no-restricted-syntax
        for (const parent of this.getParents(key)) {
          if (!isSelected) {
            this.nodes[parent].isSelected = isSelected;
            changed.set(parent, isSelected);
          }
        }
      } else {
        this.nodes[key].isSelected = isSelected;
        this.nodes[key].isIndeterminate = false;
        changed.set(key, isSelected);
      }

      // eslint-disable-next-line no-restricted-syntax
      for (const [newKey, value] of changed.entries()) {
        this.updateVnodeState(newKey);
        if (value === true) {
          this.selectedCache.add(newKey);
        } else {
          this.selectedCache.delete(newKey);
        }
      }
    },
  },
};
</script>
