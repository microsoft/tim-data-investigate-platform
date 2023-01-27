// eslint-disable-next-line import/prefer-default-export
export const createMenuContext = (items, params, ignoreCondition = false) => {
  const menu = [];
  const lookup = {};

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
  return menu;
};
