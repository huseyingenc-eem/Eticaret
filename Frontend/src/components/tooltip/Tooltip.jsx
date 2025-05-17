import React from 'react';
import { Tooltip as ReactTooltip } from 'react-tooltip';

const Tooltip = ({ id, Icon, title, bgColor }) => {
    return (
        <>
            <p data-tooltip-id={id}>
                <Icon />
            </p>
            <ReactTooltip
                id={id}
                style={{ backgroundColor: bgColor }}
                place="top"
                effect="solid"
            >
                <span className="text-sm font-medium">{title}</span>
            </ReactTooltip>
        </>
    );
};

export default Tooltip;
