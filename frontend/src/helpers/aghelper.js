// eslint-disable-next-line import/prefer-default-export
export const createMenuContext = (items, params, ignoreCondition = false) => {
  const menu = [];
  const lookup = {};
  // Propagate checked status up the menu hierarchy
  const propagateCheckedStatus = (key) => {
    const parts = key.split('-');
    parts.pop(); // Remove the last part to get the parent key
    const parentKey = parts.join('-');
    if (parentKey && lookup[parentKey]) {
      lookup[parentKey].checked = true;
      propagateCheckedStatus(parentKey); // Recursively propagate up
    }
  };

  items.forEach((item) => {
    if (typeof item === 'string') {
      menu.push(item);
      return;
    }

    const obj = {
      name: item.name,
      action: () => item.action(params),
      disabled: !ignoreCondition && typeof item.condition !== 'undefined' && !item.condition(params),
      checked: typeof item.checked !== 'undefined' && item.checked(params),
    };

    if (typeof item.path === 'undefined') {
      menu.push(obj);
      return;
    }

    let parentMenu = menu;
    let subMenuKey = 'root';
    item.path.forEach((subMenuName) => {
      subMenuKey = `${subMenuKey}-${subMenuName}`;
      if (!(subMenuKey in lookup)) {
        lookup[subMenuKey] = {
          name: subMenuName,
          subMenu: [],
        };
        parentMenu.push(lookup[subMenuKey]);
      }
      parentMenu = lookup[subMenuKey].subMenu;
    });

    if (parentMenu) {
      parentMenu.push(obj);
    }
  });

  // Check each sub-menu item and propagate checked status if necessary
  Object.keys(lookup).forEach((key) => {
    const subMenu = lookup[key].subMenu;
    if (subMenu.some(item => item.checked)) {
      lookup[key].checked = true; // Mark the current menu item as checked
      propagateCheckedStatus(key); // Propagate up the hierarchy
    }
  });

  return menu;
};
