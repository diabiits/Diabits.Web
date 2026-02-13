export function create(container, path, loop, autoplay, renderer) {
    const anim = lottie.loadAnimation({
        container,
        path,
        loop,
        autoplay,
        renderer: renderer || "svg"
    });

    return {
        dispose: () => anim.destroy()
    };
}
