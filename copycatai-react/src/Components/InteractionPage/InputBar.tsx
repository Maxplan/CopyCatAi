// Text input bar for sending requests to the server

import React, { useState } from "react";

interface InputBarProps {
    onSendMessage: (message: string) => void;
}


const InputBar: React.FC<InputBarProps> = ({ onSendMessage }) => { 
    const [input, setInput] = useState("");

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => setInput(e.target.value);

    const handleSend = () => { 
        onSendMessage(input);
        setInput("");
    };
    
    return (
        <div className="inputBar">
            <input type="text" value={input} onChange={handleInputChange} />
            <button onClick={handleSend}>Send</button>
        </div>
    )
};

export default InputBar;